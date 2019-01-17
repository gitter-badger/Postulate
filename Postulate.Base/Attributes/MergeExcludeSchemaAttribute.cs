using System;

namespace Postulate.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class MergeExcludeSchemaAttribute : Attribute
	{
		public MergeExcludeSchemaAttribute(string schema)
		{
			Schema = schema;
		}

		public string Schema { get; private set; }
	}
}