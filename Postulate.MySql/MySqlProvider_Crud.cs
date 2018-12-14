using Postulate.Base;
using Postulate.Base.Attributes;
using Postulate.Base.Extensions;
using Postulate.Base.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Postulate.MySql
{
	public partial class MySqlProvider<TKey> : CommandProvider<TKey>
	{
		public MySqlProvider(Func<object, TKey> identityConverter) : base(identityConverter, new MySqlIntegrator(), "auto_increment")
		{
		}

		protected override string ApplyDelimiter(string name)
		{
			return string.Join(".", name.Split('.').Select(s => $"`{s}`"));
		}

		protected override string FindCommand<T>(string whereClause)
		{
			var type = typeof(T);
			var props = _integrator.GetMappedColumns(type);
			var columns = props.Select(pi => new ColumnInfo(pi));
			return $"SELECT {string.Join(", ", columns.Select(col => ApplyDelimiter(col.ColumnName)))} FROM {ApplyDelimiter(_integrator.GetTableName(typeof(T)))} WHERE {whereClause}";
		}

		protected override string InsertCommand<T>()
		{
			GetInsertComponents<T>(out string columnList, out string valueList);
			return $"INSERT INTO {ApplyDelimiter(_integrator.GetTableName(typeof(T)))} ({columnList}) VALUES ({valueList}); SELECT LAST_INSERT_ID()";
		}

		protected override string PlainInsertCommand<T>(string tableName = null)
		{
			string insertTable = (string.IsNullOrEmpty(tableName)) ? _integrator.GetTableName(typeof(T)) : tableName;
			GetInsertComponents<T>(out string columnList, out string valueList);
			return $"INSERT INTO {ApplyDelimiter(insertTable)} ({columnList}) VALUES ({valueList});";
		}

		protected override string UpdateCommand<T>()
		{
			var columns = _integrator.GetEditableColumns(typeof(T), SaveAction.Update);
			return $"UPDATE {ApplyDelimiter(_integrator.GetTableName(typeof(T)))} SET {string.Join(", ", columns.Select(col => $"{ApplyDelimiter(col.ColumnName)}=@{col.PropertyName}"))} WHERE {ApplyDelimiter(typeof(T).GetIdentityName())}=@id";
		}

		protected override string DeleteCommand<T>()
		{
			return $"DELETE FROM {ApplyDelimiter(_integrator.GetTableName(typeof(T)))} WHERE {ApplyDelimiter(typeof(T).GetIdentityName())}=@id";
		}

		protected override string SqlColumnSyntax(PropertyInfo propertyInfo, bool isIdentity)
		{
			ColumnInfo col = new ColumnInfo(propertyInfo);
			string result = ApplyDelimiter(col.ColumnName);

			var calcAttr = propertyInfo.GetCustomAttribute<CalculatedAttribute>();
			if (calcAttr != null)
			{
				result += $" AS {calcAttr.Expression}";
			}
			else
			{
				string nullSyntax = (col.AllowNull) ? "NULL" : "NOT NULL";

				var typeMap = _integrator.SupportedTypes(col.Length, col.Precision, col.Scale);
				Type t = propertyInfo.GetMappedType();
				if (!typeMap.ContainsKey(t)) throw new KeyNotFoundException($"Type name {t.Name} not supported.");

				string dataType = (col.HasExplicitType()) ?
					col.DataType :
					typeMap[t].FormattedName;

				if (isIdentity) dataType += " " + IdentityColumnSyntax();

				result += $" {dataType} {nullSyntax}";
			}

			return result;
		}

		protected override string SqlSelectNextVersion(string tableName)
		{
			return $"SELECT MAX(`NextVersion`) FROM {tableName} WHERE `RecordId`=@id";
		}

		protected override string SqlUpdateNextVersion(string tableName)
		{
			return $"UPDATE {ApplyDelimiter(tableName)} SET `NextVersion`=`NextVersion`+1 WHERE `RecordId`=@id";
		}

		protected override string SqlInsertRowVersion(string tableName)
		{
			return $"INSERT INTO {ApplyDelimiter(tableName)} (`RecordId`, `NextVersion`) VALUES (@recordId, @nextVersion)";
		}

		private string UniqueIdSyntax(PropertyInfo propertyInfo)
		{
			return $"UNIQUE ({string.Join(", ", ApplyDelimiter(propertyInfo.GetColumnName()))})";
		}

		private string PrimaryKeySyntax(IEnumerable<PropertyInfo> pkColumns)
		{
			return $"PRIMARY KEY ({string.Join(", ", pkColumns.Select(pi => ApplyDelimiter(pi.GetColumnName())))})";
		}

		public override string CreateSchemaCommand(string schemaName)
		{
			return $"CREATE SCHEMA `{schemaName}`";
		}

		public override bool SchemaExists(IDbConnection connection, string schemaName)
		{
			return connection.Exists("`information_schema`.`schemata` WHERE `schema_name`=@schema", new { schema = schemaName });
		}

		protected override string CreateTableScript(TableInfo table, Type modelType)
		{
			return CreateTableCommandInner(modelType, table.ToString(), requireIdentity: false);
		}

		protected override bool TableExists(IDbConnection connection, TableInfo table)
		{
			return connection.Exists("`information_schema`.`tables` WHERE `table_name`=@table AND `table_schema`=@schema", new { table = table.Name, schema = table.Schema });
		}

		protected override string PrimaryKeySyntax(string constraintName, IEnumerable<PropertyInfo> pkColumns)
		{
			return $"PRIMARY KEY ({string.Join(", ", pkColumns.Select(pi => ApplyDelimiter(pi.GetColumnName())))})";
		}

		protected override string UniqueIdSyntax(string constraintName, PropertyInfo identityProperty)
		{
			return $"UNIQUE ({ApplyDelimiter(identityProperty.GetColumnName())}";
		}

		public override string CreateTableCommand(Type modelType)
		{
			var columns = _integrator.GetMappedColumns(modelType);
			var pkColumns = GetPrimaryKeyColumns(modelType, columns, out bool identityIsPrimaryKey);
			var identityName = modelType.GetIdentityName();

			List<string> members = new List<string>();
			members.AddRange(columns.Select(pi => SqlColumnSyntax(pi, (identityName.Equals(pi.Name)))));
			members.Add(PrimaryKeySyntax(pkColumns));
			if (!identityIsPrimaryKey) members.Add(UniqueIdSyntax(modelType.GetIdentityProperty()));

			return
				$"CREATE TABLE {ApplyDelimiter(_integrator.GetTableName(modelType))} (" +
					string.Join(",\r\n\t", members) +
				")";
		}
	}
}
