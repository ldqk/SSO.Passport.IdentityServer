using System;
using Models.Entity;

namespace Models.Dto
{
    public partial class UserPermissionOutputDto : BaseEntity
    {
        public bool HasPermission { get; set; }

        public Guid UserInfoId { get; set; }

        public int PermissionId { get; set; }
    }
}