using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("Permission")]
    public partial class Permission : BaseEntity
    {
        public Permission()
        {
            UserPermission = new HashSet<UserPermission>();
            Controls = new HashSet<Control>();
            Menu = new HashSet<Menu>();
            Role = new HashSet<Role>();
        }

        /// <summary>
        /// 权限名
        /// </summary>
        [Display(Name = "权限")]
        [Required]
        public string PermissionName { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }

        public int ClientAppId { get; set; }

        public virtual ClientApp ClientApp { get; set; }

        public virtual ICollection<UserPermission> UserPermission { get; set; }

        public virtual ICollection<Control> Controls { get; set; }

        public virtual ICollection<Menu> Menu { get; set; }

        public virtual ICollection<Role> Role { get; set; }
    }
}