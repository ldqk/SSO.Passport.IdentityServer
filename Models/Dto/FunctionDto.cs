namespace Models.Dto
{
    public class FunctionDto
    {

        public string Controller { get; set; }

        public string Action { get; set; }

        public string IconUrl { get; set; }

        public string CssStyle { get; set; }

        public string HttpMethod { get; set; }

        public string IsAvailable { get; set; }

        public string ParentId { get; set; }

        public virtual FunctionTypeDto FunctionTypeDto { get; set; }
    }
}