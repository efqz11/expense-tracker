using ExpenseTracker.App.Interfaces.Models;
using Newtonsoft.Json;

namespace ExpenseTracker.App.Models.Response
{
    public class SimpleExpense
	{
        public int Id { get; set; }
		public string Name { get; set; }
        public string Category { get; set; }

        public string Amount { get; set; }

        public string PostedAt { get; set; }

	}
}
