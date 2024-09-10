using ExpenseTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpenseTracker.App.Models.Extentions
{
    public static class LoadingScreenDbSetExtentions
	{
        public static async Task<Expense> GetWithDepsAsync(this DbSet<Expense> dbSet, Expression<Func<Expense, bool>> predicate)
        {
			Expense? existingExpense = await dbSet
					  .Include(o => o.Category)
                      .Include(o => o.User)
                      .FirstOrDefaultAsync(predicate);

            if (existingExpense == null)
            {
                throw new ApplicationException("Expense was not found in the system");
            }

            return existingExpense;

        }


    }
}
