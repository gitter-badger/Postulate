﻿using MySql.Data.MySqlClient;
using Postulate.Base.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Tests.Models
{
	[Identity(nameof(Id))]
	[TrackChanges]
	public class Item
	{
		public int Id { get; set; }

		[PrimaryKey]
		[MaxLength(100)]
		public string Name { get; set; }

		[References(typeof(ItemType))]
		public int TypeId { get; set; }

		[MaxLength(255)]
		public string Description { get; set; }

		/// <summary>
		/// Make or buy indicator
		/// </summary>
		public bool IsMade { get; set; }

		//[Column(TypeName = "money")] doesn't work in MySql
		[DecimalPrecision(10, 2)]
		public decimal Cost { get; set; }

		public DateTime? EffectiveDate { get; set; }
	}

	[Identity(nameof(Id))]
	[DereferenceId(typeof(SqlConnection), "SELECT [Name] FROM [ItemType] WHERE [Id]=@id")]
	[DereferenceId(typeof(MySqlConnection), "SELECT `Name` FROM `ItemType` WHERE `Id`=@id")]
	public class ItemType
	{
		public int Id { get; set; }

		[MaxLength(50)]
		[PrimaryKey]
		public string Name { get; set; }
	}
}