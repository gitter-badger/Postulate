using Postulate.Base.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Postulate.Base.Models
{
	public class PropertyChangeHistory<TKey>
	{
		[PrimaryKey]
		public TKey RecordId { get; set; }

		[PrimaryKey]
		public int Version { get; set; }

		[PrimaryKey]
		[MaxLength(100)]
		public string ColumnName { get; set; }

		[MaxLength(50)]
		public string UserName { get; set; }

		public DateTime DateTime { get; set; }

		public string OldValue { get; set; }
		public string NewValue { get; set; }
	}
}