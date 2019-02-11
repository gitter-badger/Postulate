using Postulate.Base;
using Postulate.Base.Attributes;

namespace Tests.Queries
{
	public class PhraseQueryTest : Query<string>
	{
		public PhraseQueryTest() : base("SELECT * FROM [Employee] {where}")
		{
		}

		[PhraseQuery("FirstName", "LastName", "Email", "Notes")]
		public string Search { get; set; }
	}
}