using System.Linq.Expressions;
using AutoMapper;
using ExpenseTracker.App.Helpers;
using ExpenseTracker.App.Interfaces.Services;
using ExpenseTracker.App.Mappers;
using ExpenseTracker.App.Models.Request;
using ExpenseTracker.App.Models.Response;
using ExpenseTracker.App.Services.Builders;
using ExpenseTracker.App.Services.Facades;
using ExpenseTracker.Data;
using ExpenseTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.App.Tests;

public class ExpenseServiceTest
{
    private readonly ExpenseService _expenseService;
    private readonly Category CATEGORY = new Category{Id = 1, Name ="Category1"};
    private readonly AppUser USER = new AppUser{Id = "be966d6e-e121-4e60-81e0-c089f59683d6", Email = "admin@mail.com", UserName = "admin@mail.com"};

    public ExpenseServiceTest()
    {
        _expenseService = GetExpenseService();
    }


    [Fact]
    public async Task AddExpense_ShouldThrowException_WhenAmountIsZeroOrNegative()
    {
        // Arrange
        ExpenseRequest expenseRequest1 = new ExpenseRequest()
        {
            Name = "Lunch",
            Amount = 0,
            CategoryId = 1,
            UserId = USER.Id,
        };


        ExpenseRequest expenseRequest2 = new ExpenseRequest()
        {
            Name = "Taxi",
            Amount = -5,
            CategoryId = 1,
            UserId = USER.Id,
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _expenseService.CreateExpenseAsync(expenseRequest1));
        await Assert.ThrowsAsync<Exception>(() => _expenseService.CreateExpenseAsync(expenseRequest2));
    }


    [Fact]
    public async Task AddExpense_ShoulCreatedNewExpense()
    {
        // Arrange
        var expense = new ExpenseRequest
        {
            Name = "Dinner",
            Amount = 50.0m,
            CategoryId = 1,
            UserId = USER.Id,
            PostedAt = DateTime.Now
        };


        // Act
        int result = await _expenseService.CreateExpenseAsync(expense);

        // Assert
        Assert.Equal(1, result);
    }


    [Fact]
    public async Task GetExpenseById_ShouldReturnExpense()
    {
        // Arrange
        var expense = new ExpenseRequest
        {
            Name = "Dinner",
            Amount = 50.0m,
            UserId = USER.Id,
            CategoryId = 1,
            PostedAt = DateTime.Now
        };

        int createdExpenseId = await _expenseService.CreateExpenseAsync(expense);

        var expectedExpense = new SimpleExpense
        {
            Id = createdExpenseId,
            Name = "Dinner",
            Amount = 50.0m.ToString("N2"),
            Category = CATEGORY.Name
        };


        // Act
        var result = await _expenseService.GetExpenseByIdAsync(createdExpenseId);

        // Assert
        Assert.Equal(expectedExpense.Amount, result.Amount);
        Assert.Equal(expectedExpense.Category, result.Category);
        Assert.Equal(expectedExpense.Name, result.Name);
    }


    [Fact]
    public async Task EditExpense_ShouldReturnUpdated()
    {
        // Arrange
        var expense = new ExpenseRequest
        {
            Name = "Dinner",
            Amount = 50.0m,
            CategoryId = 1,
            UserId = USER.Id,
            PostedAt = DateTime.Now
        };

        int createdExpenseId = await _expenseService.CreateExpenseAsync(expense);

        var expectedExpense = new SimpleExpense
        {
            Id = createdExpenseId,
            Name = "Lunch",
            Amount = 100.0m.ToString("N2"),
            Category = CATEGORY.Name,
        };


        // Act
        var updateRequest = new ExpenseUpdateRequest
        {
            Id = createdExpenseId,
            Name = "Lunch",
            Amount = 100.0m,
            CategoryId = 1,
        };
        await _expenseService.UpdateExpenseAsync(updateRequest);

        var updatedExpense = await _expenseService.GetExpenseByIdAsync(createdExpenseId);

        // Assert
        Assert.Equal(expectedExpense.Amount, updatedExpense.Amount);
        Assert.Equal(expectedExpense.Category, updatedExpense.Category);
        Assert.Equal(expectedExpense.Name, updatedExpense.Name);

        Assert.NotEqual(expense.Name, updatedExpense.Name);
        Assert.NotEqual(expense.Amount?.ToString("N2"), updatedExpense.Amount);
    }



    [Fact]
    public async Task RemoveExpense_ShouldReturnError()
    {
        // Arrange
        var expense = new ExpenseRequest
        {
            Name = "Dinner",
            Amount = 50.0m,
            CategoryId = 1,
            UserId = USER.Id,
            PostedAt = DateTime.Now
        };

        int deletingExpenseId = await _expenseService.CreateExpenseAsync(expense);

        // Act
        await _expenseService.DeleteExpenseAsync(deletingExpenseId);

        // Assert
        await Assert.ThrowsAsync<Exception>(() => _expenseService.GetExpenseByIdAsync(deletingExpenseId));
    }


    private ExpenseTrackerDbContext GetInMemoryDbContext()
    {
        // SQLite in-memory database, why? supports transactions
        var options = new DbContextOptionsBuilder<ExpenseTrackerDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new ExpenseTrackerDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        context.Categories.Add(CATEGORY);
        context.AppUsers.Add(USER);
        context.SaveChanges();

        return context;
    }

    private Mock<ILogger<T>> GetMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    private IMapper GetMapper()
    {

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ExpenseDtoMappingProfile>();
            cfg.AddProfile<SimpleExpenseProfile>();
        });

        return config.CreateMapper();
    }

    private Mock<IConfiguration> GetMockConfiguration()
    {
        var mockConfig = new Mock<IConfiguration>();

        // Set up configuration values
        mockConfig.Setup(config => config["AppSettings:Setting1"]).Returns("Value1");
        mockConfig.Setup(config => config["AppSettings:Setting2"]).Returns("Value2");

        return mockConfig;
    }
    private Mock<IPermissionService> GetMockPermissionService()
    {
        var permissionService = new Mock<IPermissionService>();

        var allowedUserIds = new[] { USER.Id };

        permissionService.Setup(service => service.GetValidUserIdToAccess(null)).Returns(USER.Id);
        permissionService.Setup(service => service.GetValidUserIdToAccess(USER.Id)).Returns(USER.Id);
        permissionService.Setup(service => service.GetAllowedUserIds()).Returns(allowedUserIds);


        // Set up the mock behavior
        permissionService.Setup(service => service.AddUsersAllowedForModify(
            It.IsAny<Expression<Func<Expense, bool>>>(),
            It.IsAny<Expression<Func<Expense, bool>>>()
        ))
        .Returns((Expression<Func<Expense, bool>> condition, Expression<Func<Expense, bool>> userSelectFunc) =>
        {
            var parameter = Expression.Parameter(typeof(Expense), "x");
            var newConditionBody = ParameterReplacer.Do(condition.Body, condition.Parameters[0], parameter);
            var userSelectFuncBody = ParameterReplacer.Do(userSelectFunc.Body, userSelectFunc.Parameters[0], parameter);

            var body = Expression.AndAlso(newConditionBody, userSelectFuncBody);
            var lambda = Expression.Lambda<Func<Expense, bool>>(body, parameter);

            return lambda;
        });

        return permissionService;
    }

    private (MainFacade<T>, BuilderFacade) GetFacades<T>()
    {
        var dbContext = GetInMemoryDbContext();

        var configuration = GetMockConfiguration();

        var mapper = GetMapper();

        var logger = GetMockLogger<T>();

        Mock<IPermissionService> permissionService = GetMockPermissionService();

        var mainFacade = new MainFacade<T>(dbContext, mapper, logger.Object, configuration.Object, permissionService.Object);


        var loggerBuilder = GetMockLogger<BuilderFacade>();

        var builderFacade = new BuilderFacade(dbContext, mapper, loggerBuilder.Object);

        return (mainFacade, builderFacade);
    }


    private ExpenseService GetExpenseService()
    {
        var (mainFacade, builderFacade) = GetFacades<ExpenseService>();

        var mockExpenseBuilder = new ExpenseBuilder(builderFacade);

        var expenseService = new ExpenseService(mainFacade, mockExpenseBuilder);

        return expenseService;
    }

}