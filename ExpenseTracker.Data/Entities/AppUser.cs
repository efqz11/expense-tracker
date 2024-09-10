using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTracker.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Data.Entities
{
    public class AppUser : IdentityUser,  ITimeTrackable
	{
		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
    }
}

