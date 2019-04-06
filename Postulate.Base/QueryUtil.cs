using Dapper;
using Postulate.Base.Attributes;
using Postulate.Base.Classes;
using Postulate.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Postulate.Base
{
	public static class QueryUtil
	{
		public static string ResolveSql(string sql, object parameters = null)
		{
			return ResolveSql(sql, parameters, out DynamicParameters queryParams);
		}

		public static string ResolveSql(string sql, object parameters, out DynamicParameters queryParams)
		{
			if (parameters == null)
			{
				queryParams = null;
				return RemoveTokens(sql);
			}

			string result = sql;

			if (HasJoinToken(result)) result = ResolveJoins(result, parameters);

			List<string> terms = new List<string>();
			var queryProps = GetProperties(parameters, result, out IEnumerable<string> builtInParams);

			queryParams = new DynamicParameters();
			foreach (var prop in queryProps)
			{
				var value = prop.GetValue(parameters);
				if (value != null && !prop.HasAttribute<PhraseAttribute>()) queryParams.Add(prop.Name, value);
			}

			var whereBuilder = GetWhereTokens();

			string token;
			if (FindWhereToken(result, out token))
			{
				// loop through this query's properties, but ignore base properties (like ResolvedSql and TraceCallback) since they are never part of WHERE clause
				foreach (var pi in queryProps)
				{
					object value = pi.GetValue(parameters);
					if (HasValue(value))
					{
						// built-in params are not part of the WHERE clause, so they are excluded from added terms
						if (!builtInParams.Contains(pi.Name.ToLower()))
						{
							var cases = pi.GetCustomAttributes(typeof(CaseAttribute), false).OfType<CaseAttribute>();
							var selectedCase = cases?.FirstOrDefault(c => c.Value.Equals(value));
							if (selectedCase != null)
							{
								terms.Add(selectedCase.Expression);
							}
							else
							{
								WhereAttribute whereAttr = pi.GetAttribute<WhereAttribute>();
								if (whereAttr != null)
								{
									terms.Add(whereAttr.Expression);
								}
								else
								{
									PhraseAttribute phraseAttr = pi.GetAttribute<PhraseAttribute>();
									if (phraseAttr != null)
									{
										var phraseQuery = new PhraseQuery(pi.Name, value.ToString(), phraseAttr.ColumnNames, phraseAttr.LeadingColumnDelimiter, phraseAttr.EndingColumnDelimiter);
										queryParams.AddDynamicParams(phraseQuery.Parameters);
										terms.Add(phraseQuery.Expression);
									}
								}
							}
						}
					}
				}

				result = ResolveWhereToken(token, result, terms);
			}			

			// remove any leftover tokens (i.e. {orderBy}, {join} etc)
			var matches = Regex.Matches(result, "(?<!{)({[^{\r\n]*})(?!{)");
			foreach (Match match in matches) result = result.Replace(match.Value, string.Empty);

			return result;
		}

		private static string ResolveJoins(string sql, object parameters)
		{
			var joinTerms = parameters.GetType().GetProperties()
				.Where(pi => pi.HasAttribute<JoinAttribute>() && pi.GetValue(parameters).Equals(true))
				.Select(pi => pi.GetAttribute<JoinAttribute>().Sql);

			return sql.Replace(InternalStringExtensions.JoinToken, string.Join("\r\n", joinTerms));
		}

		private static bool HasJoinToken(string sql)
		{
			return sql.Contains(InternalStringExtensions.JoinToken);
		}

		public static bool FindWhereToken(string sql, out string token)
		{
			var tokens = GetWhereTokens();
			return sql.ContainsAny(tokens.Select(kp => kp.Key), out token);
		}

		public static string ResolveWhereToken(string token, string sql, IEnumerable<string> criteria)
		{
			var tokens = GetWhereTokens();
			return sql.Replace(token, (criteria.Any()) ? $"{tokens[token]} {string.Join(" AND ", criteria)}" : string.Empty);
		}

		private static Dictionary<string, string> GetWhereTokens()
		{
			return new Dictionary<string, string>()
			{
				{ InternalStringExtensions.WhereToken, "WHERE" }, // query has no WHERE clause, so it will be added
				{ InternalStringExtensions.AndWhereToken, "AND" } // query already contains a WHERE clause, we're just adding to it				
			};
		}

		private static string RemoveTokens(string sql)
		{
			string result = sql;

			var tokens = GetWhereTokens();
			foreach (var kp in tokens) result = result.Replace(kp.Key, string.Empty);

			return result;
		}

		private static bool HasValue(object value)
		{
			if (value != null)
			{
				if (value.Equals(string.Empty)) return false;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the properties of a query object based on parameters defined in a
		/// SQL statement as well as properties with Where and Case attributes
		/// </summary>
		private static IEnumerable<PropertyInfo> GetProperties(object query, string sql, out IEnumerable<string> builtInParams)
		{
			// this gets the param names within the query based on words with leading '@'
			builtInParams = sql.GetParameterNames(true).Select(p => p.ToLower());
			var builtInParamsArray = builtInParams.ToArray();

			// these are the properties of the Query that are explicitly defined and may impact the WHERE clause
			var queryProps = query.GetType().GetProperties().Where(pi =>
				pi.HasAttribute<WhereAttribute>() ||
				pi.HasAttribute<CaseAttribute>() ||
				pi.HasAttribute<PhraseAttribute>() ||
				builtInParamsArray.Contains(pi.Name.ToLower()));

			return queryProps;
		}
	}
}