using ExpenseTracker.App.Models.Request;
using ExpenseTracker.Data.Entities;
using ExpenseTracker.App.Models.Dto;

namespace ExpenseTracker.App.Interfaces.Builders
{
    public interface IExpenseBuilder
	{
		public Expense CreateValidExpense(ExpenseRequest request);
		public Expense GetValidExpense(ExpenseUpdateRequest request);
		public ExpenseDto Dto { get; set; }
	}
}
