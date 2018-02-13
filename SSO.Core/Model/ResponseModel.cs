namespace SSO.Core.Model
{
    public class ResponseModel
    {
        public bool Success { get; set; }
        public bool IsLogin { get; set; } = true;
        public string Message { get; set; }
        public object Data { get; set; }
    }
}