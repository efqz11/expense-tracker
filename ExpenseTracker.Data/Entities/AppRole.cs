using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ExpenseTracker.Data.Entities
{
    public class AppRole : IdentityRole
    {

        public AppRole(string name): base(name)
        {

        }
        [StringLength(255)]
        public string? Description { get; set; }

    }

}

