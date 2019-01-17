using System;

namespace Postulate.Base.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class TrackChangesAttribute : Attribute
	{
		public TrackChangesAttribute()
		{
		}

		public string IgnoreProperties { get; set; }
	}
}