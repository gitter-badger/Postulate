using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

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
				expressions.Add(ParamExpression(qt.IsNegated, paramName));
			});
			
			Expression = string.Join(" OR ", columnNames.Select(col => $"({string.Join(" AND ", expressions.Select(expr => $"{leadingColumnDelimiter}{col}{endingColumnDelimiter} {expr}"))})"));
		}

		private static string ParamExpression(bool isNegated, string paramName)
		{
			string expr = $"'%' + @{paramName} + '%'";
			return (!isNegated) ? $"LIKE {expr}" : $"NOT LIKE {expr}";
		}

		private static IEnumerable<PhraseQueryToken> Parse(string input)
		{
			var tokens = new List<PhraseQueryToken>();

			string AddTokens(string tempInput, MatchCollection matches, Func<Match, PhraseQueryToken> selector)
			{
				if (matches.Count == 0) return tempInput;
				tokens.AddRange(matches.OfType<Match>().Select(m => selector.Invoke(m)));
				foreach (Match m in matches) tempInput = tempInput.Replace(m.Value, string.Empty);
				return tempInput;
			};

			string Unquote(string tempInput)
			{
				return Regex.Match(tempInput, "[^-\"]").Value;
			};
			
			var quotedWords = Regex.Matches(input, "\"[^\"]*\"");
			string remainder = AddTokens(input, quotedWords, (m) => new PhraseQueryToken() { Value = Unquote(m.Value) });

			var negatedQuoted = Regex.Matches(remainder, "-\"[^\"]*\"");
			remainder = AddTokens(remainder, negatedQuoted, (m) => new PhraseQueryToken() { Value = Unquote(m.Value), IsNegated = true });

			var words = Regex.Matches(remainder, "\\w"); // broken -- doesn't match whole words
			AddTokens(remainder, words, (m) => new PhraseQueryToken() { Value = m.Value });

			return tokens;
		}
	}

	internal class PhraseQueryToken
	{
		public string Value { get; set; }
		public bool IsNegated { get; set; }
	}
}