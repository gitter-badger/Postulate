using System;

namespace Postulate.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SchemaAttribute : Attribute
	{
		public SchemaAttribute(string schemaName)
		{
			Name = schemaName;
		}

		public string Name { get; }
	}
}