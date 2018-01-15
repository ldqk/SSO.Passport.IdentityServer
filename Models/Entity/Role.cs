using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("Role")]
    public partial class Role : BaseEntity
    {
        public Role()
        {
            UserGroupPermission = new HashSet<UserGroupPermission>();
            Permission = new HashSet<Permission>();
            UserInfo = new HashSet<UserInfo>();
        }

        [Required]
        public string RoleName { get; set; }

        public string Description { get; set; }

        public virtual ICollection<UserGroupPermission> UserGroupPermission { get; set; }

        public virtual ICollection<Permission> Permission { get; set; }

        public virtual ICollection<UserInfo> UserInfo { get; set; }
    }
}
