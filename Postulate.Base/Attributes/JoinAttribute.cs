using System;

namespace Postulate.Base.Attributes
{
	/// <summary>
	/// Use this on Query<T> classes on bool properties to indicate dynamically inserted joins.	
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class JoinAttribute : Attribute
	{
		public JoinAttribute(string sql)
		{
			Sql = sql;
		}

		public string Sql { get; }
	}
}