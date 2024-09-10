using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTracker.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Data.Entities
{
    public class Category : ITimeTrackable
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

		public string Name { get; set; }

		public string? Description { get; set; }

        public string? Icon { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
    }
}

