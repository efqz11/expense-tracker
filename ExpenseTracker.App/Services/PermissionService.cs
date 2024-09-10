using ExpenseTracker.App.Helpers;
using ExpenseTracker.App.Interfaces.Services;
using ExpenseTracker.App.Models.System;
using ExpenseTracker.Data;
using ExpenseTracker.Data.Entities;
using ExpenseTracker.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Security.Claims;


namespace ExpenseTracker.App.Services
{
    public class PermissionService : IPermissionService
    {
        private List<string> _availableUserIds;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private ExpenseTrackerDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        private AppUser? _appUser;

        private IList<string> _roles;
        public AppUser? AppUser
        {
            get
            {
                if (_appUser == null)
                {
                    SetAppUser().GetAwaiter().GetResult();
                }

                return _appUser;
            }

        }

        public PermissionService(
            ExpenseTrackerDbContext context,
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PermissionService> logger
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _appUser = null;
            _availableUserIds = new List<string>();
            _roles = new List<string>();
        }

        public bool IsSuperAdmin()
        {
            return IsInRole(UserRole.superadmin);
        }
        public bool IsAdmin()
        {
            return IsInRole(UserRole.admin) || IsInRole(UserRole.superadmin);
        }
        public bool IsInRole(UserRole role)
        {
            string rolename = role.ToString();
            var context = _httpContextAccessor.HttpContext;
            return context != null && context.User.IsInRole(rolename) && _roles.Contains(rolename);
        }

        public bool TryGetUserId(out string? UserId)
        {
            UserId = null;
            var context = _httpContextAccessor.HttpContext;

            if (context?.User.HasClaim(d => d.Type == ClaimTypes.NameIdentifier) == true)
            {
                var claim = context.User.Claims.First(d => d.Type == ClaimTypes.NameIdentifier);
                UserId = claim.Value;
                return true;
            }

            return false;
        }

        // public bool HasPermissions(string actionName, out string message)
        // {
        //     message = string.Empty;
        //     _availableUserIds = new List<string>();

        //     Dictionary<string, AccessRights> Permissions = AppSettings.Get<Dictionary<string, AccessRights>>("Permissions");

        //     if (AppUser != null && Permissions.TryGetValue(actionName, out AccessRights accessRights))
        //     {

        //         if (accessRights.HasFlag(AccessRights.Admin) && IsInRole(UserRole.admin))
        //         {
        //             return true;
        //         }

        //         if (accessRights.HasFlag(AccessRights.SuperAdmin) && IsInRole(UserRole.superadmin))
        //         {
        //             return true;
        //         }

        //         var userRoles = new Dictionary<UserRole, AccessRights>
        //         {
        //             { UserRole.user,  AccessRights.CurrentUser },
        //             { UserRole.guest, AccessRights.Guest },
        //             { UserRole.client, AccessRights.Client },
        //         };

        //         foreach (var role in userRoles)
        //         {
        //             if (accessRights.HasFlag(role.Value) && IsInRole(role.Key))
        //             {
        //                 _availableUserIds.Add(AppUser.Id);
        //                 return true;
        //             }
        //         }
        //     }

        //     message = GetPermissionLogMsg(actionName);
        //     return false;
        // }

        public string GetPermissionLogMsg(string methodName)
        {
            string message = $"User can't access the {methodName} module. JsonUserData: ";
            message += GetJsonUserInfo();
            return message;
        }
        public string GetJsonUserInfo()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                return string.Empty;
            }

            var userInfo = new UserLogInfo
            {
                UserName = GetUserName(context),
                UserIpAddress = GetUserIpAddress(context),
                RequestMethod = context.Request.Method,
                RequestUrl = context.Request.Path.ToString(),
                Cookies = GetCookies(context),
                Headers = GetHeaders(context)
            };

            return JsonConvert.SerializeObject(userInfo);
        }

        private async Task SetAppUser()
        {
            if (TryGetUserId(out string? UserId) && UserId != null)
            {
                /// ToDo: Add redis Cache

                _appUser = await _userManager.Users
                         .FirstOrDefaultAsync(q => q.Id == UserId);

                if (_appUser != null)
                {
                    _roles = await _userManager.GetRolesAsync(_appUser);
                }

            }

        }

        private string GetUserName(HttpContext context)
        {
            try
            {
                return context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "NoIdentityName";
            }
            catch
            {
                return "NoIdentityName";
            }
        }

        private string GetUserIpAddress(HttpContext context)
        {
            try
            {
                return context.Connection.RemoteIpAddress.ToString();
            }
            catch
            {
                return "No IP";
            }
        }

        private Dictionary<string, string> GetCookies(HttpContext context)
        {
            return context.Request.Cookies.ToDictionary(cookie => cookie.Key, cookie => cookie.Value.ToString());
        }

        private Dictionary<string, string> GetHeaders(HttpContext context)
        {
            return context.Request.Headers.ToDictionary(header => header.Key, header => header.Value.ToString());
        }

        public string[] GetAllowedUserIds()
        {
            return _availableUserIds.ToArray();
        }

        public string GetValidUserIdToAccess(string? userId)
        {

            if (!string.IsNullOrEmpty(userId) && _availableUserIds.Contains(userId))
            {
                return userId;
            }

            if (IsAdmin())
            {
                return string.IsNullOrEmpty(userId)?AppUser.Id:userId;
            }

            if (AppUser != null)
            {
                return AppUser.Id;
            }

            throw new Exception("You do not have authorization to access this user");

        }

        public Expression<Func<T, bool>> AddUsersAllowedForModify<T>(
            Expression<Func<T, bool>> condition,
            Expression<Func<T, bool>> userSelectFunc) where T : class
        {
            string[] ids = GetAllowedUserIds();

            if (ids.Count() == 0)
            {
                return condition;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var newConditionBody = ParameterReplacer.Do(condition.Body, condition.Parameters[0], parameter);
            var userSelectFuncBody = ParameterReplacer.Do(userSelectFunc.Body, userSelectFunc.Parameters[0], parameter);

            var body = Expression.AndAlso(newConditionBody, userSelectFuncBody);
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return lambda;
        }

    }



}
