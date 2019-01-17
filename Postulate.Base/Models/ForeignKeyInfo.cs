﻿namespace Postulate.Lite.Core.Models
{
	public class ForeignKeyInfo
	{
		public ColumnInfo Parent { get; set; } = new ColumnInfo();
		public ColumnInfo Child { get; set; } = new ColumnInfo();
		public string ConstraintName { get; set; }
		public bool CascadeDelete { get; set; }
	}

	public class ForeignKeyData
	{
		public string ConstraintName { get; set; }
		public string ReferencedSchema { get; set; }
		public string ReferencedTable { get; set; }
		public string ReferencedColumn { get; set; }
		public string ReferencingSchema { get; set; }
		public string ReferencingTable { get; set; }
		public string ReferencingColumn { get; set; }
		public bool CascadeDelete { get; set; }
	}
}