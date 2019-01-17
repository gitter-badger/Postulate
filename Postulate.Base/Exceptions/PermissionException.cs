using System;

namespace Postulate.Base.Exceptions
{
	public class PermissionException : Exception
	{
		public PermissionException(string message) : base(message)
		{
		}
	}
}