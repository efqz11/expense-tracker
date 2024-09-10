using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.App.Models;
using ExpenseTracker.App.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.App.Models.Request;
using ExpenseTracker.App.Models.Response;
using ExpenseTracker.Data.Entities;

namespace ExpenseTracker.App.Controllers;

public class StatisticsController : BaseController
{
    private readonly ILogger<StatisticsController> _logger;
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(ILogger<StatisticsController> logger, IStatisticsService statisticsService)
    {
        _logger = logger;
        _statisticsService = statisticsService;
    }

    public async Task<IActionResult> Index(StatisticsRequest request)
    {
        StatisticsResponse response = await _statisticsService.GetAsync(request);

        return View(response);
    }
}
