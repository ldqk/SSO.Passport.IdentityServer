namespace Models.Dto
{
    public class UserGroupInputDto
    {
        public int Id { get; set; }

        public string GroupName { get; set; }

        public int? ParentId { get; set; }
    }
}