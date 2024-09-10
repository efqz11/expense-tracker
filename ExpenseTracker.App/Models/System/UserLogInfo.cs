namespace ExpenseTracker.App.Models.System
{
    public class UserLogInfo
    {
        public string UserName { get; set; } = string.Empty;
        public string UserIpAddress { get; set; } = string.Empty;
        public string RequestMethod { get; set; } = string.Empty;
        public string RequestUrl { get; set; } = string.Empty;
        public Dictionary<string, string> Cookies { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    }
}
