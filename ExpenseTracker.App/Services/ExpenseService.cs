using System.Linq.Expressions;
using ExpenseTracker.App.Interfaces.Builders;
using ExpenseTracker.App.Models.Dto;
using ExpenseTracker.App.Models.Enums;
using ExpenseTracker.App.Models.Extentions;
using ExpenseTracker.App.Models.Request;
using ExpenseTracker.App.Models.Response;
using ExpenseTracker.App.Services.Facades;
using ExpenseTracker.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.App.Interfaces.Services
{
    public class ExpenseService : MainFacadeBasedService<ExpenseService>, IExpenseService
    {
        private readonly IExpenseBuilder _expenseBuilder;

        public ExpenseService(MainFacade<ExpenseService> facade, IExpenseBuilder expenseBuilder) : base(facade)
        {
            _expenseBuilder = expenseBuilder;
        }

        public async Task<PaginatedResponse<SimpleExpense>> GetAllAsync(ExpenseSearchRequest request)
        {
            string userId = PermissionService.GetValidUserIdToAccess();

            IQueryable<Expense> query = Context.Expenses
                .Include(q=> q.Category)
                .Where(q=> q.UserId == userId)
                .OrderByDescending(q=> q.CreatedAt);

            if (request.HasValidDates())
            {
                query = query.Where(q=> q.PostedAt >= request.Start!.Value && q.PostedAt <= request.End!.Value);
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(q=> q.CategoryId == request.CategoryId);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(q=> EF.Functions.Like(q.Name, $"%{request.Search}%"));
            }

            int count = await query.CountAsync();

            List<Expense> response = await query.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToListAsync();

            List<SimpleExpense> expenseResponses = Mapper.Map<List<SimpleExpense>>(response);

            PaginatedResponse<SimpleExpense> paginatedResponse = new PaginatedResponse<SimpleExpense>(expenseResponses, count, request.Page, request.Limit);

            return paginatedResponse;
        }


        public async Task<SimpleExpense> GetExpenseByIdAsync(int id)
        {
            try
            {
                var existingExpense = await Context.Expenses.GetWithDepsAsync(GetItemPredicate(id));

                var expense = Mapper.Map<SimpleExpense>(existingExpense);

                return expense;
            }
            catch (Exception ex)
            {
                HandleError(ex);

                throw;
            }
        }


        public async Task<ExpenseUpdateRequest> GetExpenseByIdForEditAsync(int id)
        {
            try
            {
                var existingExpense = await Context.Expenses.GetWithDepsAsync(GetItemPredicate(id));

                var expense = Mapper.Map<ExpenseUpdateRequest>(existingExpense);

                return expense;
            }
            catch (Exception ex)
            {
                HandleError(ex);

                throw;
            }
        }


        public async Task<int> CreateExpenseAsync(ExpenseRequest expenseRequest)
        {

            await using var transaction = await Context.Database.BeginTransactionAsync();

            MainActionType type = MainActionType.Create;

            try
            {
                expenseRequest.UserId = PermissionService.GetValidUserIdToAccess(expenseRequest.UserId);

                AppUser user = PermissionService.AppUser;

                SetScreenDto(user, type);

                Expense expense = _expenseBuilder.CreateValidExpense(expenseRequest);

                Context.Expenses.Add(expense);


                var result = await Context.SaveChangesAsync();

                await transaction.CommitAsync();

                return expense.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                HandleError(ex);
            }

            return 0;
        }


        public async Task UpdateExpenseAsync(ExpenseUpdateRequest screenRequest)
        {
            await using var transaction = await Context.Database.BeginTransactionAsync();

            screenRequest.UserId = PermissionService.GetValidUserIdToAccess(screenRequest.UserId);

            MainActionType type = MainActionType.Update;

            try
            {

                Expense existingExpense = await Context.Expenses.GetWithDepsAsync(GetItemPredicate(screenRequest.Id));

                AppUser user = Mapper.Map<AppUser>(existingExpense.User);

				if (!user.IsEqualId(screenRequest.UserId) && !PermissionService.IsAdmin())
				{
					throw new ApplicationException("No authorization for this user");
				}

                SetScreenDto(user, MainActionType.Update, existingExpense);

                existingExpense = _expenseBuilder.GetValidExpense(screenRequest);


                if (!_expenseBuilder.Dto.IsChanged)
                {
                    throw new ApplicationException("Данные по экран загрузки не изменены");
                }

                Context.Expenses.Update(existingExpense);

                await Context.SaveChangesAsync();

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                HandleError(ex);
            }
        }


        public async Task DeleteExpenseAsync(long id, string? userId = null)
        {
            await using var transaction = await Context.Database.BeginTransactionAsync();

            userId = PermissionService.GetValidUserIdToAccess(userId);

            MainActionType type = MainActionType.Delete;

            try
            {

                Expense existingExpense = await Context.Expenses.GetWithDepsAsync(GetItemPredicate(id));

                AppUser user = existingExpense.User;

                if (!user.IsEqualId(userId) && !PermissionService.IsAdmin())
                {
                    throw new ApplicationException("No authorization for this user");
                }

                // TODO: soft-delete
                Context.Expenses.Remove(existingExpense);

                await Context.SaveChangesAsync();

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                HandleError(ex);
            }

        }

        public async Task<SelectList> GetCategoriesAsync(int? id = null)
        {
            var data = await Context.Categories.ToDictionaryAsync(q=> q.Id, q=> q.Name);
            return new SelectList(data, "Key", "Value", id);
        }

        private void HandleError(Exception ex)
        {
            if (ex is not ApplicationException)
            {
                Logger.LogError(ex.Message);

                throw new Exception("Failed to process your request at this time");
            }

            throw new Exception(ex.Message);
        }

        private ExpenseDto SetScreenDto(AppUser appUser, MainActionType type, Expense? expense = null)
        {
            _expenseBuilder.Dto = new ExpenseDto()
            {
                Expense = expense
            };

            return _expenseBuilder.Dto;
        }



        public Expression<Func<Expense, bool>> GetItemPredicate(long? Id)
        {
            string[] allowedUserIds = PermissionService.GetAllowedUserIds();

            Expression<Func<Expense, bool>> select_exp = o => o.Id == Id;
            Expression<Func<Expense, bool>> access_exp = o => allowedUserIds.Contains(o.UserId);

			return PermissionService.AddUsersAllowedForModify(select_exp, access_exp);
		}

    }

}
