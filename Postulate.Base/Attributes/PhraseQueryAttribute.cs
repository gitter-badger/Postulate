using System;

namespace Postulate.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class PhraseQueryAttribute : Attribute
	{
		public PhraseQueryAttribute(params string[] columnNames)
		{
			ColumnNames = columnNames;
		}

		public string[] ColumnNames { get; }
	}
}