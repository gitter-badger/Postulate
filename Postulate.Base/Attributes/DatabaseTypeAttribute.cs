using System;

namespace Postulate.Base
{
	public enum Databases
	{
		SqlServer,
		MySql
	}

	namespace Attributes
	{
		/// <summary>
		/// Specifies the type of Postulate.Base.CommandProvider will be used with model merges.
		/// Required by Postulate Merge UI
		/// </summary>
		[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
		public class DatabaseTypeAttribute : Attribute
		{
			public DatabaseTypeAttribute(Databases databaseType)
			{
				DatabaseType = databaseType;
			}

			public Databases DatabaseType { get; private set; }
		}
	}
}