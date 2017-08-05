using System;

namespace Models.Dto
{
    public class UserPermissionInputDto
    {
        public int Id { get; set; }
        public bool HasPermission { get; set; }

        public Guid? UserInfoId { get; set; }

        public int? PermissionId { get; set; }
    }
}