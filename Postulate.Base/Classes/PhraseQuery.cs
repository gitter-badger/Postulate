using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Postulate.Base.Classes
{
	internal class PhraseQuery
	{
		public IEnumerable<PhraseQueryToken> Tokens { get; }
		public DynamicParameters Parameters { get; }		
		public string Expression { get; }

		public PhraseQuery(string propertyName, string input, string[] columnNames, char leadingColumnDelimiter, char endingColumnDelimiter)
		{
			Tokens = Parse(input);
			Parameters = new DynamicParameters();
			var expressions = new List<string>();
			int index = 0;
			Tokens.ToList().ForEach((qt) =>
			{
				string paramName = $"{propertyName}Search{++index}";
				Parameters.Add(paramName, qt.Value, DbType.String);
				expressions.Add(qt.GetExpression(paramName));
			});
			
			Expression = string.Join(" OR ", columnNames.Select(col => $"({string.Join(" AND ", expressions.Select(expr => $"{leadingColumnDelimiter}{col}{endingColumnDelimiter} {expr}"))})"));
		}

		private static IEnumerable<PhraseQueryToken> Parse(string input)
		{
			throw new NotImplementedException();
		}
	}

	internal class PhraseQueryToken
	{
		public string Value { get; set; }
		public bool IsNegated { get; set; }

		internal string GetExpression(string paramName)
		{
			string result = $"'%' + @{paramName} + '%'";
			return (!IsNegated) ? $"LIKE {result}" : $"NOT LIKE {result}";
		}
	}
}