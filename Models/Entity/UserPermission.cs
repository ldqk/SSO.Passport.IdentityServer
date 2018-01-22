using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("UserPermission")]
    public partial class UserPermission : BaseEntity
    {
        public bool HasPermission { get; set; }

        [ForeignKey("UserInfo")]
        public Guid UserInfoId { get; set; }

        [ForeignKey("Permission")]
        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }

        public virtual UserInfo UserInfo { get; set; }
    }
}
