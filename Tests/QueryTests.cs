using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Postulate.Base;
using Postulate.Base.Attributes;
using System;

namespace Tests
{
	public class Criteria
	{
		[Where("[LastName] LIKE @lastName")]
		public string LastName { get; set; }

		[Case(1, "[Level]<10")]
		[Case(2, "[Level]>=10 AND [Level]<20")]
		public int Level { get; set; }
	}

	public class ResultClass
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime HireDate { get; set; }
	}

	public class ActiveEmployees : Query<ResultClass>
	{
		public ActiveEmployees() : base("SELECT * FROM [Employee] WHERE [IsActive]=@isActive {andWhere} ORDER BY [Nothing]")
		{
		}

		public bool IsActive { get; set; } = true;

		[Where("[LastName] LIKE @lastName")]
		public string LastName { get; set; }

		[Case(1, "[Level]<10")]
		[Case(2, "[Level]>=10 AND [Level]<20")]
		public int Level { get; set; }
	}

	[TestClass]
	public class QueryTests
	{
		[TestMethod]
		public void ResolveSqlWithWhere()
		{
			string query = "SELECT * FROM [Whatever] {where} ORDER BY [Nothing]";
			string result = QueryUtil.ResolveSql(query, new Criteria() { LastName = "hello" });
			Assert.IsTrue(result.Equals("SELECT * FROM [Whatever] WHERE [LastName] LIKE @lastName ORDER BY [Nothing]"));
		}

		[TestMethod]
		public void ResolveSqlWithCase()
		{
			string query = "SELECT * FROM [Whatever] {where} ORDER BY [Nothing]";
			string result = QueryUtil.ResolveSql(query, new Criteria() { Level = 2 });
			Assert.IsTrue(result.Equals("SELECT * FROM [Whatever] WHERE [Level]>=10 AND [Level]<20 ORDER BY [Nothing]"));
		}

		[TestMethod]
		public void ResolveSqlNoCriteria()
		{
			string query = "SELECT * FROM [Whatever] {where} ORDER BY [Nothing]";
			string result = QueryUtil.ResolveSql(query);
			Assert.IsTrue(result.Equals("SELECT * FROM [Whatever]  ORDER BY [Nothing]"));
		}

		[TestMethod]
		public void ResolveSqlWithQuery()
		{
			var query = new ActiveEmployees() { LastName = "hello" };
			string sql = QueryUtil.ResolveSql(query.Sql, query);
			Assert.IsTrue(sql.Equals("SELECT * FROM [Employee] WHERE [IsActive]=@isActive AND [LastName] LIKE @lastName ORDER BY [Nothing]"));
		}

		[TestMethod]
		public void ResolveSqlWithParams()
		{
			var query = new ActiveEmployees() { LastName = "hello", IsActive = true };
			string sql = QueryUtil.ResolveSql(query.Sql, query, out DynamicParameters queryParams);
			Assert.IsTrue(queryParams.Get<string>("LastName").Equals("hello"));
			Assert.IsTrue(queryParams.Get<bool>("IsActive").Equals(true));
		}

		[TestMethod]
		public void ResolveSqlNoParams()
		{
			var query = new ActiveEmployees();
			string sql = QueryUtil.ResolveSql(query.Sql, query);
			Assert.IsTrue(sql.Equals("SELECT * FROM [Employee] WHERE [IsActive]=@isActive  ORDER BY [Nothing]"));
		}
	}
}