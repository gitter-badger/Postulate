using Dapper;
using System;
using System.Runtime.CompilerServices;

namespace Postulate.Base.Classes
{
	public class QueryTrace
	{
		public QueryTrace(string sql, DynamicParameters parameters, TimeSpan duration, [CallerMemberName]string methodName = null)
		{
			MethodName = methodName;
			Sql = sql;
			Parameters = parameters;
			Duration = duration;			
		}

		public string MethodName { get; }
		public string Sql { get; }
		public DynamicParameters Parameters { get; }
		public TimeSpan Duration { get; }
	}
}