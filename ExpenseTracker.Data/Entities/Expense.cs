using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpenseTracker.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data.Entities
{
    public class Expense : ITimeTrackable
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
		public string? Name { get; set; }

        public Category Category { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public AppUser User { get; set; }

        [Required]
        public string UserId { get; set; }

        [Precision(8, 2)]
        public decimal Amount { get; set; }

		public DateTime PostedAt { get; set; } = DateTime.Now;

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
    }
}

