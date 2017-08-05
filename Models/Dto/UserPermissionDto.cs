using System;

namespace Models.Dto
{
    public class UserPermissionDto
    {
        public int Id { get; set; }

        public bool HasPermission { get; set; }

        public virtual PermissionDto PermissionDto { get; set; }
    }
}