using Postulate.Base;
using Postulate.Base.Extensions;
using Postulate.Base.Models;
using System;
using System.Data;

namespace Postulate.SqlServer
{
	public partial class SqlServerProvider<TKey> : CommandProvider<TKey>
	{
		public override bool SchemaExists(IDbConnection connection, string schemaName)
		{
			return connection.Exists("[sys].[schemas] WHERE [name]=@name", new { name = schemaName });
		}

		protected override bool TableExists(IDbConnection connection, TableInfo table)
		{
			return connection.Exists("[sys].[tables] WHERE SCHEMA_NAME([schema_id])=@schema AND [name]=@name", table);
		}

		protected override string CreateTableScript(TableInfo table, Type modelType)
		{
			return CreateTableCommandInner(modelType, table.ToString(), requireIdentity: false);
		}

		protected override string SqlSelectNextVersion(string tableName)
		{
			return $"SELECT MAX([NextVersion]) FROM {tableName} WHERE [RecordId]=@id";
		}

		protected override string SqlUpdateNextVersion(string tableName)
		{
			return $"UPDATE {tableName} SET [NextVersion]=[NextVersion]+1 WHERE [RecordId]=@id";
		}

		protected override string SqlInsertRowVersion(string tableName)
		{
			return $"INSERT INTO {tableName} ([RecordId], [NextVersion]) VALUES (@recordId, @nextVersion)";
		}
	}
}