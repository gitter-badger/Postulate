using System;
using System.Collections.Generic;
using System.Text;

namespace Postulate.Base.Models
{
	public class ChangeHistory<TKey>
	{
		public TKey RecordId { get; set; }
		public int Version { get; set; }
		public DateTime DateTime { get; set; }
		public string UserName { get; set; }
		public IEnumerable<PropertyChange> Properties { get; set; }
	}
}
