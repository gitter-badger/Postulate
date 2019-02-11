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
				string paramName = $"{propertyName}{++index}";
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

			string AddTokens(string tempInput, IEnumerable<string> matches, Func<string, PhraseQueryToken> selector)
			{
				if (matches.Count() == 0) return tempInput;
				tokens.AddRange(matches.Select(s => selector.Invoke(s)));
				foreach (string m in matches) tempInput = tempInput.Replace(m, string.Empty);
				return tempInput;
			};

			string Unquote(string tempInput)
			{
				return Regex.Match(tempInput, "[^-\"]").Value;
			};

			IEnumerable<string> GetMatches(string inputInner, string pattern)
			{
				return Regex.Matches(inputInner, pattern).OfType<Match>().Select(m => m.Value);
			}
			
			var quotedWords = GetMatches(input, "\"[^\"]*\"");
			string remainder = AddTokens(input, quotedWords, (s) => new PhraseQueryToken() { Value = Unquote(s) });

			var negatedQuoted = GetMatches(remainder, "-\"[^\"]*\"");
			remainder = AddTokens(remainder, negatedQuoted, (s) => new PhraseQueryToken() { Value = Unquote(s), IsNegated = true });

			var words = remainder.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
			AddTokens(remainder, words, (m) => PhraseQueryToken.FromString(m));

			return tokens;
		}
	}

	internal class PhraseQueryToken
	{
		public string Value { get; set; }
		public bool IsNegated { get; set; }

		public static PhraseQueryToken FromString(string input)
		{
			return (input.StartsWith("-")) ?
				new PhraseQueryToken() { Value = input.Substring(1), IsNegated = true } :
				new PhraseQueryToken() { Value = input };
		}
	}
}