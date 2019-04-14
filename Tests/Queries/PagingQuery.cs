using Postulate.Base;
using Postulate.Base.Attributes;
using System;

namespace Tests.Queries
{
	public class DateRangeResult
	{
		public DateTime Date { get; set; }
		public int DayOfWeek { get; set; }
		public int Flag { get; set; }
		public int WeekNumber { get; set; }
		public long? RowNumber { get; set; }
	}

	/// <summary>
	/// This is a sample query from Ginseng8, not meant to be executed from Postulate.
	/// We're just going to assert against the resolved SQL, and I needed a fully-defined query
	/// </summary>
	public class PagingQuery : Query<DateRangeResult>
	{
		public PagingQuery() : base(
			@"SELECT
				[dr].*,
				ROW_NUMBER() OVER (ORDER BY [Date]) AS [RowNumber]
			FROM
				dbo.FnDateRange('4/1/19', '5/1/19') [dr]
			ORDER BY
				[Date]
			{offset}")
		{
		}

		[Offset(10)]
		public int? Page { get; set; }
	}
}