using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using ExpenseTracker.Data.Entities;

namespace ExpenseTracker.Data
{

    public class DefaultInitializer: IDefaultInitializer
    {
        private readonly ILogger<DefaultInitializer> _logger;
        private readonly ExpenseTrackerDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly Dictionary<string,string> roles;

        public DefaultInitializer(ILogger<DefaultInitializer> logger, ExpenseTrackerDbContext ctx, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _logger = logger;
            _context = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            CultureInfo culture = CultureInfo.InvariantCulture; // Or new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

        }

        public async Task InitializeAsync()
        {
            string adminName = "administrator";
            string adminEmail = "admin@mail.com";
            string password = "_Aa123456";
            Dictionary<string , string> expenses = new Dictionary<string, string>()
            {
                {"Utilities", "fa fa-meed"},
                {"Subscriptions", "fa fa-meed"},
                {"Housing", "fa fa-meed"},
                {"Cellphone", "fa fa-meed"},
                {"Entertainment", "fa fa-meed"},
                {"Insurance", "fa fa-meed"},
                {"Clothes", "fa fa-meed"},
                {"Healthcare", "fa fa-meed"},
                {"Retirement", "fa fa-meed"},
                {"Emergency fund", "fa fa-meed"},
                {"Home maintenance", "fa fa-meed"},
                {"Pet care", "fa fa-meed"},
                {"Restaurants", "fa fa-meed"},
                {"Savings", "fa fa-meed"},
                {"Furnishings", "fa fa-meed"},
                {"Gifts", "fa fa-meed"},
                {"Vehicle insurance", "fa fa-meed"},
                {"Gas", "fa fa-meed"},
                {"Food", "fa fa-meed"},
                {"Travel", "fa fa-meed"},
                {"Debt", "fa fa-meed"},
                {"Transportation", "fa fa-meed"},
                {"Car payment", "fa fa-meed"},
                {"Mortgage", "fa fa-meed"},
            };


            Dictionary<string, AppRole> roles = new Dictionary<string, AppRole>
            {
                 {
                    "superadmin",
                    new AppRole("superadmin")
                    {
                        Description = "Super admininstrator for app"
                    }
                 },
                 {
                    "admin",
                    new AppRole("admin")
                    {
                        Description = "Administrator for app"
                    }
                },
               {
                    "user",
                    new AppRole("user")
                    {
                        Description = "User for app"
                    }
                },
                {
                    "guest",
                    new AppRole("guest")
                    {
                        Description = "Guest user for app"
                    }
                },
            };

            foreach (var role in roles)
            {
                if (await _roleManager.FindByNameAsync(role.Key) == null)
                {
                    await _roleManager.CreateAsync(role.Value);
                }
            }


            foreach (var expense in expenses.Keys)
            {
                Category cat = new Category() { Name = expense };

                _context.Categories.Add(cat);
            }

            await _context.SaveChangesAsync();


            var superadmin = await _userManager.FindByEmailAsync(adminEmail);


            if (superadmin == null)
            {
                AppUser admin = new AppUser { Email = adminEmail, UserName = adminEmail, FirstName = adminName };
                IdentityResult result = await _userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "admin");
                }
            }
            else
            {
                var superadminRoles = await _userManager.GetRolesAsync(superadmin);

                if (!superadminRoles.Contains("superadmin") || !await _userManager.IsInRoleAsync(superadmin, "superadmin"))
                {
                    await _userManager.AddToRoleAsync(superadmin, "superadmin");
                }

            }

        }

    }

}
