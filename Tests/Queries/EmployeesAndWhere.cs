using Postulate.Base;
using Postulate.Base.Attributes;
using Tests.Models;

namespace Tests.Queries
{
	public class EmployeesAndWhere : Query<EmployeeInt>
	{
		public EmployeesAndWhere() : base("SELECT * FROM [dbo].[Employee] WHERE [IsActive]=1 {andWhere} ORDER BY [LastName]")
		{
		}

		[Where("[LastName] LIKE @lastName")]
		public string LastName { get; set; }
	}
}