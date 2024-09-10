using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ExpenseTracker.App.Models.Response;

public class StatisticsResponse
{
    public DateTime Previous { get; set; }

    public DateTime Next { get; set; }

    public DateTime CurrentMonth { get; set; }

    public Dictionary<string, decimal[]> Data { get; set; }


}
