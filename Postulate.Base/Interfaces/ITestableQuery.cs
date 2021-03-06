﻿using System.Collections.Generic;
using System.Data;

namespace Postulate.Base.Interfaces
{
	/// <summary>
	/// Implement this on your Query types to make it easy to unit test your queries.
	/// Use <see cref="Query{TResult}.TestExecuteHelper(IDbConnection)"/> in your interface implementation
	/// </summary>
	public interface ITestableQuery
	{
		IEnumerable<dynamic> TestExecute(IDbConnection connection);
	}
}