using ExpenseTracker.Data.Entities;

namespace ExpenseTracker.App.Models.Dto
{
    public class ExpenseDto
	{
		public long ProjectId { get; set; }

		public string Name { get; set; }

        public int CategoryId { get; set; }

        public string UserId { get; set; }

        public decimal Amount { get; set; }

		public DateTime? PostedAt { get; set; }


		public Expense? Expense { get; set; }


		public bool IsChanged { get; set; } = false;

	}
}

