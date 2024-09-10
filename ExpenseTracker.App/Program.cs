using System.Globalization;
using ExpenseTracker.App.Interfaces.Builders;
using ExpenseTracker.App.Interfaces.Services;
using ExpenseTracker.App.Services;
using ExpenseTracker.App.Services.Builders;
using ExpenseTracker.App.Services.Facades;
using ExpenseTracker.Data;
using ExpenseTracker.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// // Configure MySQL
// builder.Services.AddDbContext<ExpenseTrackerDbContext>(options =>
//     options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!));


// Replace with your connection string.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


var serverVersion = ServerVersion.AutoDetect(connectionString);

// Add Db context, detect version.
builder.Services.AddDbContext<ExpenseTrackerDbContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion)
        // The following three options help with debugging, but should
        // be changed or removed for production.
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);


builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6; // CU-8693maz6c _01.02.2024
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ExpenseTrackerDbContext>()
.AddDefaultUI() // Enable default UI
.AddDefaultTokenProviders();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(12);
	// options.Cookie.HttpOnly = true;
    options.Cookie.Name = ".gaadiya.food"; // <--- Add line
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IDefaultInitializer, DefaultInitializer>();


// Facades
builder.Services.AddScoped(typeof(MainFacade<>));
builder.Services.AddScoped<BuilderFacade>();

builder.Services.AddScoped<IPermissionService, PermissionService>();

builder.Services.AddScoped<IExpenseBuilder, ExpenseBuilder>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

builder.Services.AddScoped<IStatisticsService, StatisticsService>();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}


// // TODO: Move to new migration project
// try
// {
//     using (var scope = app.Services.CreateScope())
//     {
//         var dbInitializer = scope.ServiceProvider.GetService<IDefaultInitializer>();
//         await dbInitializer.InitializeAsync();
//     }
// }
// catch (Exception ex)
// {
//     // Handle the exception
//     Console.WriteLine($"migration completed with error: {ex.Message}");
// }

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// This is necessary for Identity UI to work
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// ambiguous between the following methods or properties:

// 'Microsoft.Extensions.DependencyInjection.IdentityServiceCollectionExtensions.AddIdentity<TUser, TRole>(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action<Microsoft.AspNetCore.Identity.IdentityOptions>)'

//  and


// 'Microsoft.Extensions.DependencyInjection.IdentityServiceCollectionExtensions.AddIdentity<TUser, TRole>(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action<Microsoft.AspNetCore.Identity.IdentityOptions>)'