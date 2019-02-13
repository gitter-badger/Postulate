using Postulate.Base;
using Postulate.Base.Attributes;
using Tests.Models;

namespace Tests.Queries
{
	public class EmployeeEmptyWhere : Query<EmployeeInt>
	{
		public EmployeeEmptyWhere() : base("SELECT * FROM [dbo].[Employee] {where} ORDER BY [LastName]")
		{
		}

		[Case(true, "[IsActive]=1")]
		public bool ActiveOnly { get; set; }		

		[Where("[LastName] LIKE @lastName")]
		public string LastName { get; set; }
	}
}