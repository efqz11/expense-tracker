using ExpenseTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExpenseTracker.App.Models.Extentions
{
    public static class AppUserSetExtentions
	{

        public static bool IsEqualId(this AppUser user, string id)
        {
            if (user.Id == id)
            {
                return true;
            }

            return false;
        }

    }
}
