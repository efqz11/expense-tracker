using ExpenseTracker.Data.Entities;
using ExpenseTracker.Data.Enums;
using System.Linq.Expressions;

namespace ExpenseTracker.App.Interfaces.Services
{
    public interface IPermissionService
    {
        public bool IsSuperAdmin();
        public bool IsAdmin();
        public bool IsInRole(UserRole role);
        // public bool HasPermissions(string methodName, out string message);
        public bool TryGetUserId(out string? UserId);
        public AppUser AppUser { get; }
        public string GetJsonUserInfo();
        public string[] GetAllowedUserIds();
        public string GetValidUserIdToAccess(string? userId = null);
        public Expression<Func<T, bool>> AddUsersAllowedForModify<T>(
           Expression<Func<T, bool>> condition,
           Expression<Func<T, bool>> userSelectFunc) where T : class;


     }
}
