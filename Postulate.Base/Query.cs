using Dapper;
using Postulate.Base.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Postulate.Base
{
	/// <summary>
	/// Encapsulates a SQL SELECT query with dynamic criteria that returns TResult
	/// </summary>
	public class Query<TResult>
	{
		public Query(string sql)
		{
			Sql = sql;
		}

		public string Sql { get; private set; }
		public string ResolvedSql { get; private set; }
		public DynamicParameters Parameters { get; private set; }

		public string ResolveSql()
		{			
			ResolvedSql = QueryUtil.ResolveSql(Sql, this, out DynamicParameters queryParams);
			Parameters = queryParams;
			return ResolvedSql;
		}

		public string ResolveSql(out DynamicParameters queryParams)
		{
			ResolvedSql = QueryUtil.ResolveSql(Sql, this, out queryParams);
			Parameters = queryParams;
			return ResolvedSql;
		}

		public IEnumerable<TResult> Execute(IDbConnection connection)
		{
			ResolvedSql = QueryUtil.ResolveSql(Sql, this, out DynamicParameters queryParams);
			Parameters = queryParams;

			try
			{
				return connection.Query<TResult>(ResolvedSql, queryParams);
			}
			catch (Exception exc)
			{
				throw new QueryException(exc, ResolvedSql, queryParams);
			}
		}

		public TResult ExecuteSingle(IDbConnection connection)
		{
			ResolvedSql = QueryUtil.ResolveSql(Sql, this, out DynamicParameters queryParams);
			Parameters = queryParams;

			try
			{
				return connection.QuerySingle<TResult>(ResolvedSql, queryParams);
			}
			catch (Exception exc)
			{
				throw new QueryException(exc, ResolvedSql, queryParams);
			}
		}

		public async Task<TResult> ExecuteSingleAsync(IDbConnection connection)
		{
			ResolvedSql = QueryUtil.ResolveSql(Sql, this, out DynamicParameters queryParams);
			Parameters = queryParams;

			try
			{
				return await connection.QuerySingleAsync<TResult>(ResolvedSql, queryParams);
			}
			catch (Exception exc)
			{
				throw new QueryException(exc, ResolvedSql, queryParams);
			}
		}

		public async Task<IEnumerable<TResult>> ExecuteAsync(IDbConnection connection)
		{
			ResolvedSql = QueryUtil.ResolveSql(Sql, this, out DynamicParameters queryParams);
			Parameters = queryParams;

			try
			{
				return await connection.QueryAsync<TResult>(ResolvedSql, queryParams);
			}
			catch (Exception exc)
			{
				throw new QueryException(exc, ResolvedSql, queryParams);
			}
		}

		/// <summary>
		/// Intended for implementing <see cref="Interfaces.ITestableQuery"/> for unit testing, not intended for use on its own
		/// </summary>
		public IEnumerable<dynamic> TestExecuteHelper(IDbConnection connection)
		{
			try
			{
				ResolvedSql = QueryUtil.ResolveSql(Sql, this, out DynamicParameters queryParams);
				return connection.Query(ResolvedSql, queryParams);
			}
			catch (Exception exc)
			{
				throw new Exception($"Query {GetType().Name} failed: {exc.Message}", exc);
			}
		}
	}
}