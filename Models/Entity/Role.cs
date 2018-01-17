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
            UserGroupPermission = new HashSet<UserGroupRole>();
            Permission = new HashSet<Permission>();
            UserInfo = new HashSet<UserInfo>();
        }

        /// <summary>
        /// 角色名
        /// </summary>
        [Required]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }


        [Display(Name = "父级id")]
        public int? ParentId { get; set; }

        public virtual ICollection<Role> Children { get; set; }

        public virtual Role Parent { get; set; }

        public virtual ICollection<ClientApp> ClientApp { get; set; }

        public virtual ICollection<UserGroupRole> UserGroupPermission { get; set; }

        public virtual ICollection<Permission> Permission { get; set; }

        public virtual ICollection<UserInfo> UserInfo { get; set; }
    }
}