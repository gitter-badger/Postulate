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
		public string[] Terms { get; }
		public string Expression { get; }

		public PhraseQuery(string propertyName, string input)
		{
			Tokens = Parse(input);

			Parameters = new DynamicParameters();
			var termList = new List<string>();
			int index = 0;
			Tokens.ToList().ForEach((qt) =>
			{
				string paramName = $"{propertyName}Search{++index}";
				Parameters.Add(paramName, qt.Value, DbType.String);
				termList.Add(qt.GetExpression(paramName));
			});

			Terms = termList.ToArray();
			Expression = string.Join(" OR ", $"({string.Join(" AND ", Terms)})");
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