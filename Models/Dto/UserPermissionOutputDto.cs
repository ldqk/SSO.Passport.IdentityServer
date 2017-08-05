namespace Models.Dto
{
    public class UserPermissionOutputDto
    {
        public int Id { get; set; }

        public bool HasPermission { get; set; }

        public virtual PermissionOutputDto PermissionOutputDto { get; set; }
    }
}