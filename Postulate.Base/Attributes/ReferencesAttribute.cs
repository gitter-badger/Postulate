using System;

namespace Postulate.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ReferencesAttribute : Attribute
	{
		public ReferencesAttribute(Type primaryType)
		{
			PrimaryType = primaryType;
		}

		public Type PrimaryType { get; private set; }

		/// <summary>
		/// When creating this relationship, enable cascade deletes
		/// </summary>
		public bool CascadeDelete { get; set; }
	}
}