using ExpenseTracker.App.Interfaces.Models;
using Newtonsoft.Json;

namespace ExpenseTracker.App.Models.Response
{
    public class PaginatedResponse<T> : IPaginatedResponse where T : class
	{
		public int CurrentPage { get; set; }
		public int PageCount { get; set; }
		public int? PreviousPage { get; set; }
		public int? NextPage { get; set; }
		public List<T> Results { get; set; }
		public int ResultCount { get; set; }
        public int TotalCount { get; set; }
		public Dictionary<string, object> Data { get; set; }

		public PaginatedResponse(List<T> data, int totalItemCount, int page, int limit, Dictionary<string, object> jsonData = null)
		{
			this.Data = jsonData;
			this.Results = data;
			this.ResultCount = data.Count();
			this.CurrentPage = page;
            this.TotalCount = totalItemCount;

			if (page > 1)
				this.PreviousPage = page - 1;

			this.PageCount = Convert.ToInt32(Math.Ceiling((decimal)totalItemCount / limit));
			if (page < PageCount)
				this.NextPage = page + 1;
		}

		public void SetData<TData>(TData model) {
			var _json = JsonConvert.SerializeObject(model);
			this.Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(_json);
			// PropertyInfo[] infos = model.GetType().GetProperties();

			// Dictionary<string, object> dix = new Dictionary<string, object>();

			// foreach (PropertyInfo info in infos)
			// {
			// 	dix.Add(info.Name, info.GetValue(model, null));
			// }

			// this.Data = dix;
		}
	}
}
