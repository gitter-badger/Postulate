using Postulate.Base;
using Postulate.Base.Attributes;
using Tests.Models;

namespace Tests.Queries
{
	public class EmployeeEmptyWhere : Query<EmployeeInt>
	{
		public EmployeeEmptyWhere() : base("SELECT [e].* FROM [dbo].[Employee] [e] {join} {where} ORDER BY [e].[LastName]")
		{
		}

		[Case(true, "[IsActive]=1")]
		public bool ActiveOnly { get; set; }

		[Where("[LastName] LIKE @lastName")]
		public string LastName { get; set; }

		[Join("INNER JOIN [dbo].[Organization] [org] ON [e].[OrganizationId]=[org].[Id]")]
		public bool WithOrgs { get; set; }
	}
}