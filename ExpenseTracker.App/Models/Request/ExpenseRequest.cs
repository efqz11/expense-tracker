using System.ComponentModel.DataAnnotations;
using ExpenseTracker.App.Attribute;

namespace ExpenseTracker.App.Models.Request;

public class ExpenseRequest
{
    public string? UserId { get; set; }

    public string? Name { get; set; }

    public DateTime? PostedAt { get; set; }

    public int CategoryId { get; set; }

    [ValidateAmount(isRequired: true)]
    public decimal? Amount { get; set; }
}
