using Postulate.Base;
using Postulate.Base.Attributes;
using Postulate.Base.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Threading.Tasks;

namespace Tests.Models
{
	[Identity(nameof(Id), IdentityPosition.FirstColumn)]
	[Table("Employee")]
	public class EmployeeInt : Record, IFindRelated<int>
	{
		public static DateTime DefaultDate = new DateTime(1900, 1, 1);

		[References(typeof(Organization))]
		public int OrganizationId { get; set; }

		public Organization Organization { get; set; }

		[MaxLength(50)]
		[Required]
		public string FirstName { get; set; }

		[MaxLength(50)]
		[Required]
		public string LastName { get; set; }

		[MaxLength(50)]
		public string Email { get; set; }

		public string Notes { get; set; }

		[Column(TypeName = "date")]
		public DateTime? HireDate { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime DateCreated { get; set; } = DefaultDate;

		public DateTime? DateModified { get; set; }

		public int Id { get; set; }

		public void FindRelated(IDbConnection connection, CommandProvider<int> commandProvider)
		{
			Organization = commandProvider.Find<Organization>(connection, OrganizationId);
		}

		public override bool CheckFindPermission(IDbConnection connection, IUser user)
		{
			return (!user.UserName.Equals("adamo"));
		}

		public override bool CheckDeletePermission(IDbConnection connection, IUser user)
		{
			return (!user.UserName.Equals("adamo"));
		}

		public override bool CheckSavePermission(IDbConnection connection, IUser user)
		{
			return (!user.UserName.Equals("adamo"));
		}

		public const string InvalidMessage = "That was too long ago";

		public override bool Validate(IDbConnection connection, out string message)
		{
			message = null;

			if (HireDate < new DateTime(1970, 1, 1))
			{
				message = InvalidMessage;
				return false;
			}

			return true;
		}

		public override async Task BeforeSaveAsync(IDbConnection connection, SaveAction action, IUser user)
		{
			DateTime timestamp = user?.LocalTime ?? DateTime.UtcNow;

			switch (action)
			{
				case SaveAction.Insert:
					await Task.Run(() => { DateCreated = timestamp; });
					break;

				case SaveAction.Update:
					await Task.Run(() => { DateModified = timestamp; });
					break;
			}
		}
	}

	[Identity(nameof(Id))]
	[Table("Employee")]
	public class EmployeeLong : Record, IFindRelated<long>
	{
		[References(typeof(Organization))]
		public int OrganizationId { get; set; }

		public Organization Organization { get; set; }

		[MaxLength(50)]
		[Required]
		public string FirstName { get; set; }

		[MaxLength(50)]
		[Required]
		public string LastName { get; set; }

		[MaxLength(50)]
		public string Email { get; set; }

		[Column(TypeName = "date")]
		public DateTime? HireDate { get; set; }

		public bool IsActive { get; set; } = true;

		public long Id { get; set; }

		public void FindRelated(IDbConnection connection, CommandProvider<long> commandProvider)
		{
			Organization = commandProvider.Find<Organization>(connection, OrganizationId);
		}
	}

	[Identity(nameof(Id))]
	[Table("Employee")]
	public class EmployeeGuid : Record
	{
		[References(typeof(Organization))]
		public int OrganizationId { get; set; }

		public Organization Organization { get; set; }

		[MaxLength(50)]
		[Required]
		public string FirstName { get; set; }

		[MaxLength(50)]
		[Required]
		public string LastName { get; set; }

		[MaxLength(50)]
		public string Email { get; set; }

		[Column(TypeName = "date")]
		public DateTime? HireDate { get; set; }

		public bool IsActive { get; set; } = true;

		public Guid Id { get; set; }
	}
}