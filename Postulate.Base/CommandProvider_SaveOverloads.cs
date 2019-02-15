using Dapper;
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
		private CommandDefinition BuildUpdateCommand<TModel>(TModel @object, IEnumerable<string> propertyNames, string tableName = null)
		{
			DynamicParameters dp = GetDynamicParameters(@object, propertyNames);
			dp.Add("id", GetIdentity(@object));
			return new CommandDefinition(UpdateCommand<TModel>(tableName, propertyNames), dp);
		}

		private CommandDefinition BuildInsertCommand<TModel>(TModel @object, IEnumerable<string> propertyNames, string tableName = null)
		{
			DynamicParameters dp = GetDynamicParameters(@object, propertyNames);
			return new CommandDefinition(InsertCommand<TModel>(tableName, propertyNames), dp);
		}

		private static DynamicParameters GetDynamicParameters(object @object, IEnumerable<string> propertyNames)
		{
			Type modelType = @object.GetType();
			DynamicParameters dp = new DynamicParameters();
			propertyNames.ToList().ForEach((col) =>
			{
				PropertyInfo pi = modelType.GetProperty(col);
				var value = pi.GetValue(@object);
				dp.Add(col, value);
			});
			return dp;
		}

		/// <summary>
		/// Inserts or updates a record, setting only the given propertyNames
		/// </summary>
		public async Task<TKey> SaveAsync<TModel>(IDbConnection connection, TModel @object, params string[] propertyNames)
		{
			if (IsNew(@object))
			{
				var cmd = BuildInsertCommand(@object, propertyNames);
				object id = await connection.ExecuteScalarAsync(cmd);
				SetIdentity(@object, ConvertIdentity(id));
			}
			else
			{
				var cmd = BuildUpdateCommand(@object, propertyNames);
				await connection.ExecuteScalarAsync(cmd);
			}

			return GetIdentity(@object);
		}

		/// <summary>
		/// Inserts or updates a record, setting only the given propertyNames
		/// </summary>
		public TKey Save<TModel>(IDbConnection connection, TModel @object, params string[] propertyNames)
		{
			if (IsNew(@object))
			{
				var cmd = BuildInsertCommand(@object, propertyNames);
				object id = connection.ExecuteScalar(cmd);
				SetIdentity(@object, ConvertIdentity(id));
			}
			else
			{
				var update = BuildUpdateCommand(@object, propertyNames);
				connection.Execute(update);
			}

			return GetIdentity(@object);
		}
	}
}