using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.App.Models.Request;

public class ExpenseSearchRequest
{
    public string? Search { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    public int? CategoryId { get; set; }

    public int Page { get; set; } = 1;

    public int Limit { get; set; } = 10;

    public bool HasValidDates() => Start != null && End != null;
}
