﻿using Postulate.Base.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Postulate.Base.Interfaces
{
	public interface ITrackedRecord
	{
		/// <summary>
		/// Indicates whether changes are saved to the default history table
		/// </summary>
		bool UseDefaultHistoryTable { get; }

		/// <summary>
		/// Enables custom save behavior for property changes
		/// </summary>
		void TrackChanges(IDbConnection connection, int version, IEnumerable<PropertyChange> changes, IUser user);

		Task TrackChangesAsync(IDbConnection connection, int version, IEnumerable<PropertyChange> changes, IUser user);
	}
}