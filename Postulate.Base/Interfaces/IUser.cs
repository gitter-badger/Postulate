using System;

namespace Postulate.Base.Interfaces
{
	public interface IUser
	{
		string UserName { get; }
		DateTime LocalTime { get; }
	}
}