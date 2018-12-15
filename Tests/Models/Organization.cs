using Postulate.Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Tests.Models
{
	[Identity(nameof(Id), IdentityPosition.LastColumn)]
	public class Organization
	{
		[MaxLength(50)]
		[PrimaryKey]
		public string Name { get; set; }

		public int? EmployeeCount { get; set; }

		public int Id { get; set; }
	}
}