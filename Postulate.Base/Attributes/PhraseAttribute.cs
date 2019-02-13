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
	}
}