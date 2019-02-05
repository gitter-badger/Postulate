using System.Data;
using System.Threading.Tasks;

namespace Postulate.Base.Interfaces
{
	/// <summary>
	/// Implement this on model types that require foreign key lookups whenever a record is accessed
	/// </summary>
	public interface IFindRelated<TKey>
	{
		/// <summary>
		/// Use this to set navigation properties whenever a record is accessed
		/// </summary>
		void FindRelated(IDbConnection connection, CommandProvider<TKey> commandProvider);

		Task FindRelatedAsync(IDbConnection connection, CommandProvider<TKey> commandProvider);
	}
}