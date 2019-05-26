using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
	public abstract class BaseEntity
	{
		public int Id { get; set; }
		public DateTime? Created { get; set; }

		public string CreatedBy { get; set; }

		public DateTime? Updated { get; set; }

		public string UpdatedBy { get; set; }

		public bool? IsDisabled { get; set; }
	}
}
