using System;

namespace Postulate.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class PrimaryKeyAttribute : Attribute
	{
	}
}