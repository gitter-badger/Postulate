using Dapper;
using Postulate.Base.Attributes;
using Postulate.Base.Extensions;
using Postulate.Base.Interfaces;
using Postulate.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Postulate.Base
{
	public abstract partial class CommandProvider<TKey>
	{
		private bool IsTrackingChanges<TModel>(out string[] ignoreProperties)
		{
			ignoreProperties = null;
			var attr = typeof(TModel).GetAttribute<TrackChangesAttribute>();
			if (attr == null) return false;

			ignoreProperties = (!string.IsNullOrEmpty(attr.IgnoreProperties)) ?
				attr.IgnoreProperties.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray() :
				Enumerable.Empty<string>().ToArray();
			return true;
		}

		const string changesSchema = "changes";

		protected abstract bool TableExists(IDbConnection connection, TableInfo table);
		protected abstract string CreateTableScript(TableInfo table, Type modelType);

		/// <summary>
		/// Query that returns the max version number for a given RecordId (must use parameter @id)
		/// </summary>
		protected abstract string SqlSelectNextVersion(string tableName);

		protected abstract string SqlUpdateNextVersion(string tableName);

		protected abstract string SqlInsertRowVersion(string tableName);

		public IEnumerable<ChangeHistory<TKey>> QueryChangeHistory<TModel>(IDbConnection connection, TKey id, int timeZoneOffset = 0)
		{
			TableInfo obj = Syntax.GetTableInfoFromType(typeof(TModel));
			string tableName = $"{obj.Schema}_{obj.Name}";


			var results = connection.Query<PropertyChangeHistory<TKey>>(
				$@"SELECT * FROM [{_changesSchema}].[{tableName}] WHERE [RecordId]=@id ORDER BY [DateTime] DESC", new { id });

			return results.GroupBy(item => new
			{
				item.RecordId,
				item.Version
			}).Select(ch =>
			{
				return new ChangeHistory<TKey>()
				{
					RecordId = ch.Key.RecordId,
					DateTime = ch.First().DateTime.AddHours(timeZoneOffset),
					Version = ch.Key.Version,
					UserName = ch.First().UserName,
					Properties = ch.Select(chr => new PropertyChange()
					{
						PropertyName = chr.ColumnName,
						OldValue = chr.OldValue,
						NewValue = chr.NewValue
					})
				};
			});
		}

		private async Task<IEnumerable<PropertyChange>> GetChangesAsync<TModel>(IDbConnection connection, TModel @object)
		{
			if (IsTrackingChanges<TModel>(out string[] ignoreProperties))
			{
				var existing = await FindAsync<TModel>(connection, GetIdentity(@object));
				if (existing != null) return GetPropertyChanges(connection, existing, @object, ignoreProperties).ToArray();
			}
			return null;
		}

		private IEnumerable<PropertyChange> GetChanges<TModel>(IDbConnection connection, TModel @object)
		{
			if (IsTrackingChanges<TModel>(out string[] ignoreProperties))
			{
				var existing = Find<TModel>(connection, GetIdentity(@object));
				if (existing != null) return GetPropertyChanges(connection, existing, @object, ignoreProperties).ToArray();
			}
			return null;
		}

		private async Task SaveChangesAsync<TModel>(IDbConnection connection, TModel @object, IEnumerable<PropertyChange> changes, IUser user)
		{
			if (!changes?.Any() ?? true) return;

			var trackedRecord = @object as ITrackedRecord;
			bool useHistoryTable = trackedRecord?.UseDefaultHistoryTable ?? true;
			VerifyChangeTrackingObjects<TModel>(connection, useHistoryTable, out string historyTableName);

			TKey identity = GetIdentity(@object);
			int version = await IncrementNextRecordVersionAsync<TModel>(connection, identity);

			if (useHistoryTable)
			{
				foreach (var change in changes)
				{
					PropertyChangeHistory<TKey> history = GetChangeHistoryRecord(identity, user, version, change);
					await PlainInsertAsync(connection, history, tableName: historyTableName);
				}
			}

			trackedRecord?.TrackChanges(connection, version, changes, user);
		}

		private void SaveChanges<TModel>(IDbConnection connection, TModel @object, IEnumerable<PropertyChange> changes, IUser user)
		{
			if (!changes?.Any() ?? true) return;

			var trackedRecord = @object as ITrackedRecord;
			bool useHistoryTable = trackedRecord?.UseDefaultHistoryTable ?? true;
			VerifyChangeTrackingObjects<TModel>(connection, useHistoryTable, out string historyTableName);

			TKey identity = GetIdentity(@object);
			int version = IncrementNextRecordVersion<TModel>(connection, identity);

			if (useHistoryTable)
			{
				foreach (var change in changes)
				{
					PropertyChangeHistory<TKey> history = GetChangeHistoryRecord(identity, user, version, change);
					PlainInsert(connection, history, tableName: historyTableName);
				}
			}

			trackedRecord?.TrackChanges(connection, version, changes, user);
		}

		private static PropertyChangeHistory<TKey> GetChangeHistoryRecord(TKey identity, IUser user, int version, PropertyChange change)
		{
			return new PropertyChangeHistory<TKey>()
			{
				RecordId = identity,
				Version = version,
				ColumnName = change.PropertyName,
				UserName = user?.UserName ?? "<unknown>",
				DateTime = user?.LocalTime ?? DateTime.UtcNow,
				OldValue = CleanMinDate(change.OldValue)?.ToString() ?? "<null>",
				NewValue = CleanMinDate(change.NewValue)?.ToString() ?? "<null>"
			};
		}

		private static object CleanMinDate(object value)
		{
			// prevents DateTime.MinValue from getting passed to SQL Server as a parameter, where it fails
			if (value is DateTime && value.Equals(default(DateTime))) return null;
			return value;
		}

		private async Task<int> IncrementNextRecordVersionAsync<TModel>(IDbConnection connection, TKey identity)
		{
			var table = _integrator.GetTableInfo(typeof(TModel));
			var versionTbl = GetVersionTableInfo(table, changesSchema);

			var param = new { id = identity };
			string tableName = versionTbl.ToString();

			int result = await connection.QuerySingleOrDefaultAsync<int?>(SqlSelectNextVersion(tableName), param) ?? 0;

			if (result == 0)
			{
				RowVersion<TKey> initialVersion = new RowVersion<TKey>()
				{
					RecordId = identity,
					NextVersion = 1
				};
				await PlainInsertAsync(connection, initialVersion, tableName: tableName);
				result = 1;
			}

			await connection.ExecuteAsync(SqlUpdateNextVersion(tableName), param);

			return result;
		}

		private int IncrementNextRecordVersion<TModel>(IDbConnection connection, TKey identity)
		{
			var table = _integrator.GetTableInfo(typeof(TModel));
			var versionTbl = GetVersionTableInfo(table, changesSchema);

			var param = new { id = identity };
			string tableName = versionTbl.ToString();

			int result = connection.QuerySingleOrDefault<int?>(SqlSelectNextVersion(tableName), param) ?? 0;

			if (result == 0)
			{
				RowVersion<TKey> initialVersion = new RowVersion<TKey>()
				{
					RecordId = identity,
					NextVersion = 1
				};
				PlainInsert(connection, initialVersion, tableName: tableName);
				result = 1;
			}

			connection.Execute(SqlUpdateNextVersion(tableName), param);

			return result;
		}

		private void VerifyChangeTrackingObjects<TModel>(IDbConnection connection, bool createHistoryTable, out string historyTableName)
		{
			historyTableName = null;
			var targetTable = _integrator.GetTableInfo(typeof(TModel));

			if (!SchemaExists(connection, changesSchema)) connection.Execute(CreateSchemaCommand(changesSchema));

			if (createHistoryTable)
			{
				TableInfo historyTbl = new TableInfo(changesSchema, $"{targetTable.Schema}_{targetTable.Name}_History");
				historyTableName = historyTbl.ToString();
				CreateTableIfNotExists(connection, historyTbl, typeof(PropertyChangeHistory<TKey>));
			}

			TableInfo versionTbl = GetVersionTableInfo(targetTable, changesSchema);
			CreateTableIfNotExists(connection, versionTbl, typeof(RowVersion<TKey>));
		}

		private static TableInfo GetVersionTableInfo(TableInfo table, string changesSchema)
		{
			return new TableInfo(changesSchema, $"{table.Schema}_{table.Name}_Version");
		}

		private void CreateTableIfNotExists(IDbConnection connection, TableInfo table, Type modelType)
		{
			if (!TableExists(connection, table))
			{
				var script = CreateTableScript(table, modelType);
				connection.Execute(script);
			}
		}

		private IEnumerable<PropertyChange> GetPropertyChanges<TModel>(IDbConnection connection, TModel currentRecord, TModel newRecord, string[] ignoreProperties)
		{
			var properties = _integrator.GetEditableColumns(typeof(TModel), SaveAction.Update);
			return properties
				.Where(col => !ignoreProperties.Contains(col.PropertyName))
				.Select(col => new PropertyChange()
				{
					PropertyName = col.PropertyName,
					OldValue = GetPropertyValue(connection, col.PropertyInfo, currentRecord),
					NewValue = GetPropertyValue(connection, col.PropertyInfo, newRecord)
				}).Where(pc => pc.IsChanged());
		}

		private object GetPropertyValue<TModel>(IDbConnection connection, PropertyInfo propertyInfo, TModel record)
		{
			object result = propertyInfo.GetValue(record);
			if (result == null) return null;
			if (GetNameFromId(connection, propertyInfo, result, out string name)) result = name;
			return result;
		}

		private bool GetNameFromId(IDbConnection connection, PropertyInfo propertyInfo, object id, out string name)
		{
			var fk = propertyInfo.GetAttribute<ReferencesAttribute>();
			if (fk != null)
			{
				var instance = Activator.CreateInstance(fk.PrimaryType);
				var lookup = instance as INameLookup;
				if (lookup != null)
				{
					string idCol = fk.PrimaryType.GetIdentityName();
					string tableName = GetTableName(fk.PrimaryType, null);
					string query = $"SELECT {lookup.NameExpression} FROM {ApplyDelimiter(tableName)} WHERE {ApplyDelimiter(idCol)}=@id";
					try
					{
						name = connection.QuerySingle<string>(query, new { id });
						return true;
					}
					catch (Exception exc)
					{
						throw new Exception($"Tried to lookup a Name in table {tableName} for Id value {id}, but an error occurred: {exc.Message}");
					}
				}
			}

			name = null;
			return false;
		}
	}
}