using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using ExpenseTracker.Data.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{

    public interface IDefaultInitializer
    {
        Task InitializeAsync();
    }

}
