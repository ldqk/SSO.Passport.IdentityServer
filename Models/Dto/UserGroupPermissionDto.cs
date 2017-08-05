namespace Models.Dto
{
    public class UserGroupPermissionDto
    {
        public int Id { get; set; }

        public bool HasPermission { get; set; }

        public int UserGroupId { get; set; }

        public int RoleId { get; set; }

        public virtual RoleDto RoleDto { get; set; }
    }
}