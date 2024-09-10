using ExpenseTracker.App.Models.Request;
using ExpenseTracker.App.Models.Response;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseTracker.App.Interfaces.Services
{
    public interface IStatisticsService
    {
        Task<StatisticsResponse> GetAsync(StatisticsRequest request);
    }

}
