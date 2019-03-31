using Postulate.Base.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace Postulate.Base
{
	public abstract partial class Record
	{
		/// <summary>
		/// Override this to apply any changes to a record immediately before it's saved, such as audit tracking fields
		/// </summary>
		public virtual Task BeforeSaveAsync(IDbConnection connection, SaveAction action, IUser user)
		{
			// do nothing by default
			return Task.CompletedTask;
		}

		/// <summary>
		/// Override this to execute logic after a record is successfully saved
		/// </summary>
		public virtual Task AfterSaveAsync(IDbConnection connection, SaveAction action, IUser user)
		{
			// do nothing by default
			return Task.CompletedTask;
		}

		public virtual Task BeforeDeleteAsync(IDbConnection connection)
		{
			// do nothing by default
			return Task.CompletedTask;
		}

		public virtual Task AfterDeleteAsync(IDbConnection connection)
		{
			// do nothing by default
			return Task.CompletedTask;
		}
	}
}