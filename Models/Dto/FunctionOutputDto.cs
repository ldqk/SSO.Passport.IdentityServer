using Models.Enum;

namespace Models.Dto
{
    public class FunctionOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string IconUrl { get; set; }

        public string CssStyle { get; set; }

        public string HttpMethod { get; set; }

        public bool IsAvailable { get; set; }

        public int ParentId { get; set; }
        public FunctionType FunctionType { get; set; }
    }
}