namespace Models.ViewModel
{
    public class PageDataViewModel
    {
        public object Data { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
    }
}