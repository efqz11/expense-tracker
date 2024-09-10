using ExpenseTracker.App.Models.Request;
using ExpenseTracker.App.Models.Response;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.App.Interfaces.Services
{
    public interface IExpenseService
    {
         public Task<PaginatedResponse<SimpleExpense>> GetAllAsync(ExpenseSearchRequest request);
         public Task<SimpleExpense> GetExpenseByIdAsync(int id);
         public Task<ExpenseUpdateRequest> GetExpenseByIdForEditAsync(int id);
         public Task<int> CreateExpenseAsync(ExpenseRequest expenseRequest);
         public Task UpdateExpenseAsync(ExpenseUpdateRequest screenRequest);
         public Task DeleteExpenseAsync(long id, string? userId = null);
        public Task<SelectList> GetCategoriesAsync(int? id = null);
    }

}
