using System;

namespace Postulate.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class FullTextAttribute : Attribute
	{
		public FullTextAttribute(params string[] columnNames)
		{
			ColumnNames = columnNames;
		}

		public string[] ColumnNames { get; }
	}
}