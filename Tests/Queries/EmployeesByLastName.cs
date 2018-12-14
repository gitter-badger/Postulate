using Postulate.Base;
using Postulate.Base.Attributes;
using Tests.Models;

namespace Tests.Queries
{
	public class EmployeesByLastName : Query<EmployeeInt>
	{
		public EmployeesByLastName(string query) : base(query)
		{
		}

		[Where("LastName LIKE @lastName")]
		public string LastName { get; set; }
	}
}