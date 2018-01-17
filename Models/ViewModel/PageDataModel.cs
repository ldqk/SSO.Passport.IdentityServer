using System;

namespace Models.ViewModel
{

    public class PageDataModel
    {
        public PageDataModel(object data, int pageCount, int totalCount)
        {
            this.Data = data;
            this.PageCount = pageCount;
            this.TotalCount = totalCount;
        }

        public Object Data { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }

    }

    public class ResponseModel
    {
        public bool Success { get; set; }
        public bool IsLogin { get; set; } = true;
        public string Message { get; set; }
        public object Data { get; set; }
    }
}