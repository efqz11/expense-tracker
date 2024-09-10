
namespace ExpenseTracker.App.Interfaces.Models
{
	public interface IPaginatedResponse
	{
		public int CurrentPage { get; set; }
		public int PageCount { get; set; }
		public int? PreviousPage { get; set; }
		public int? NextPage { get; set; }
		public int ResultCount { get; set; }
        public int TotalCount { get; set; }
    }
}