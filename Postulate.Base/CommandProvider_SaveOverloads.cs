using Postulate.Base.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Postulate.Base
{
	public abstract partial class CommandProvider<TKey>
	{
		/// <summary>
		/// Inserts or creates TTarget from explicitly set properties in TSource and returns the inserted or updated id value
		/// </summary>
		public async Task<TKey> SaveAsync<TTarget, TSource>(IDbConnection connection, TKey identity, TSource source, Action<TTarget> mapping, IUser user = null, string tableName = null) where TTarget : new()
		{
			TTarget target = (identity.Equals(default(TKey))) ?
				new TTarget() :
				await FindAsync<TTarget>(connection, identity, user, tableName);

			mapping.Invoke(target);

			return await SaveAsync(connection, target, user, tableName);
		}

		/// <summary>
		/// Inserts or creates TTarget from explicitly set properties in TSource and returns the inserted or updated id value
		/// </summary>
		public TKey Save<TTarget, TSource>(IDbConnection connection, TKey identity, TSource source, Action<TTarget> mapping, IUser user = null, string tableName = null) where TTarget : new()
		{
			TTarget target = (identity.Equals(default(TKey))) ?
				new TTarget() :
				Find<TTarget>(connection, identity, user, tableName);

			mapping.Invoke(target);

			return Save(connection, target, user, tableName);
		}
	}
}