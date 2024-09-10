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
    public class StatisticsService : MainFacadeBasedService<StatisticsService>, IStatisticsService
    {
        public StatisticsService(MainFacade<StatisticsService> facade) : base(facade)
        {
        }

        public async Task<StatisticsResponse> GetAsync(StatisticsRequest request)
        {
            request.Date ??= DateTime.Now;

            DateTime compareDate = request.Date.Value;

            string userId = PermissionService.GetValidUserIdToAccess();

            Dictionary<string, decimal[]> data = await Context.Expenses
                .Include(q=> q.Category)
                .Where(q=> q.UserId == userId
                    && q.PostedAt.Month == compareDate.Month
                    && q.PostedAt.Year == compareDate.Year)
                .GroupBy(q=> q.Category.Name)
                .ToDictionaryAsync(q=> q.Key, q=> new decimal[] { q.Sum(c=> c.Amount), q.Count() });

            StatisticsResponse response = new StatisticsResponse();

            response.CurrentMonth = request.Date.Value!;

            response.Next = request.Date.Value!.AddMonths(1).Date;

            response.Previous = request.Date.Value!.AddMonths(-1).Date;

            response.Data = data;


            return response;
        }

    }

}
