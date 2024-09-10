using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.App.Models;
using ExpenseTracker.App.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.App.Models.Request;
using ExpenseTracker.App.Models.Response;
using ExpenseTracker.Data.Entities;

namespace ExpenseTracker.App.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IExpenseService _expenseService;

    public HomeController(ILogger<HomeController> logger, IExpenseService expenseService)
    {
        _logger = logger;
        _expenseService = expenseService;
    }

    public async Task<IActionResult> Index(ExpenseSearchRequest request)
    {
        PaginatedResponse<SimpleExpense> expenses = await _expenseService.GetAllAsync(request);

        ViewBag.CategoryDropdown = await _expenseService.GetCategoriesAsync(request.CategoryId);

        SetFilterViewBags(request);

        return View(expenses);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.CategoryDropdown = await _expenseService.GetCategoriesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _expenseService.CreateExpenseAsync(request);

                SetTempDataMessage("New expense record was added", Models.Enums.MsgAlertType.success);

                return RedirectToAction(nameof(Index));
            }
        }
        catch (System.Exception ex)
        {
            SetTempDataMessage(ex.Message);
        }


        if (!ModelState.IsValid)
        {
            string? error = GetModelStateErrors(ModelState).FirstOrDefault();

            SetTempDataMessage(error, Models.Enums.MsgAlertType.danger);
        }

        ViewBag.CategoryDropdown = await _expenseService.GetCategoriesAsync(request.CategoryId);

        return View(request);
    }


    public async Task<IActionResult> Edit(int id)
    {
        ExpenseUpdateRequest expenseUpdateRequest = await _expenseService.GetExpenseByIdForEditAsync(id);

        ViewBag.CategoryDropdown = await _expenseService.GetCategoriesAsync(expenseUpdateRequest.CategoryId);

        return View(expenseUpdateRequest);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ExpenseUpdateRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _expenseService.UpdateExpenseAsync(request);

                SetTempDataMessage("Expense record was updated", Models.Enums.MsgAlertType.success);

                return RedirectToAction(nameof(Index));
            }
        }
        catch (System.Exception ex)
        {
            SetTempDataMessage(ex.Message);
        }


        if (!ModelState.IsValid)
        {
            string? error = GetModelStateErrors(ModelState).FirstOrDefault();

            SetTempDataMessage(error, Models.Enums.MsgAlertType.danger);
        }

        ViewBag.CategoryDropdown = await _expenseService.GetCategoriesAsync(request.CategoryId);

        return View(request);
    }



    public async Task<IActionResult> Remove(int id)
    {
        SimpleExpense expense = await _expenseService.GetExpenseByIdAsync(id);
        return View(expense);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveConfirm(int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _expenseService.DeleteExpenseAsync(id);

                SetTempDataMessage("Expense record was removed", Models.Enums.MsgAlertType.success);

                return RedirectToAction(nameof(Index));
            }
        }
        catch (System.Exception ex)
        {
            SetTempDataMessage(ex.Message);
        }


        if (!ModelState.IsValid)
        {
            string? error = GetModelStateErrors(ModelState).FirstOrDefault();

            SetTempDataMessage(error, Models.Enums.MsgAlertType.danger);
        }

        return RedirectToAction(nameof(Remove), new { id });
    }

    private void SetFilterViewBags(ExpenseSearchRequest request)
    {
        ViewBag.ExpenseSearchRequest = request;

        ViewBag.Search = request.Search;
        ViewBag.CategoryId = request.CategoryId;
        ViewBag.Start = request.Start;
        ViewBag.End = request.End;
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
