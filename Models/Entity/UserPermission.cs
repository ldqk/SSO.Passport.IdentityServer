using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("UserPermission")]
    public class UserPermission
    {
        public UserPermission()
        {
            HasPermission = true;
        }
        [Key]
        public int Id { get; set; }

        public bool HasPermission { get; set; }

        [Required, ForeignKey("UserInfo")]
        public Guid UserInfoId { get; set; }

        [Required, ForeignKey("Permission")]
        public int PermissionId { get; set; }

        public virtual Permission Permission { get; set; }

        public virtual UserInfo UserInfo { get; set; }
    }
}