using Postulate.Base.Attributes;

namespace Postulate.Base.Models
{
	/// <summary>
	/// Used with change tracking to get the next version number of a record when saving change history
	/// </summary>
	public class RowVersion<TKey>
	{
		[PrimaryKey]
		public TKey RecordId { get; set; }

		public int NextVersion { get; set; }
	}
}