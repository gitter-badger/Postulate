using System;

namespace Postulate.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class PhraseAttribute : Attribute
	{
		public PhraseAttribute(params string[] columnNames)
		{
			ColumnNames = columnNames;
		}

		public string[] ColumnNames { get; }

		public char LeadingColumnDelimiter { get; set; } = '[';
		public char EndingColumnDelimiter { get; set; } = ']';
	}
}