using Dapper;
using Postulate.Base.Extensions;
using Postulate.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Postulate.Base
{
	public abstract partial class CommandProvider<TKey>
	{
		private CommandDefinition BuildUpdateCommand<TModel>(TModel @object, IEnumerable<string> columnNames, string tableName = null)
		{
			Type modelType = typeof(TModel);
			DynamicParameters dp = new DynamicParameters();
			string setColumns = string.Join(", ", columnNames.Select(col =>
			{
				PropertyInfo pi = modelType.GetProperty(col);
				var value = pi.GetValue(@object);
				dp.Add(col, value);
				return $"{ApplyDelimiter(col)}=@{col}";
			}));

			return new CommandDefinition($"UPDATE {ApplyDelimiter(GetTableName(modelType, tableName))} SET {setColumns} WHERE {ApplyDelimiter(modelType.GetIdentityName())}=@id", dp);
		}

		public async Task<TKey> SaveAsync<TModel>(IDbConnection connection, TModel @object, IUser user, params string[] columnNames)
		{
			if (IsNew(@object) || columnNames == null)
			{
				return await SaveAsync(connection, @object, user);
			}
			else
			{
				var update = BuildUpdateCommand(@object, columnNames, null);
				await connection.ExecuteAsync(update);
				return GetIdentity(@object);
			}
		}

		public TKey Save<TModel>(IDbConnection connection, TModel @object, IUser user, params string[] columnNames)
		{
			if (IsNew(@object) || columnNames == null)
			{
				return Save(connection, @object, user);
			}
			else
			{
				var update = BuildUpdateCommand(@object, columnNames, null);
				connection.Execute(update);
				return GetIdentity(@object);
			}
		}

		public async Task<TKey> SaveAsync<TModel>(IDbConnection connection, TModel @object, IUser user, params Expression<Func<TModel, object>>[] setColumns)
		{
			if (IsNew(@object) || setColumns == null)
			{
				return await SaveAsync(connection, @object, user);
			}
			else
			{
				await UpdateAsync(connection, @object, user, setColumns);
				return GetIdentity(@object);
			}
		}

		public TKey Save<TModel>(IDbConnection connection, TModel @object, IUser user, params Expression<Func<TModel, object>>[] setColumns)
		{ 
			if (IsNew(@object) || setColumns == null)
			{
				return Save(connection, @object, user);
			}
			else
			{
				Update(connection, @object, user, setColumns);
				return GetIdentity(@object);
			}
		}
	}
}