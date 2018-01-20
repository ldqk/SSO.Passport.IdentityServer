using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("ClientApp")]
    public partial class ClientApp : BaseEntity
    {
        public ClientApp()
        {
            UserGroup = new HashSet<UserGroup>();
            UserInfo = new HashSet<UserInfo>();
            Controls = new HashSet<Control>();
            Menus = new HashSet<Menu>();
            Permissions = new HashSet<Permission>();
            Roles = new HashSet<Role>();
            Available = true;
        }

        /// <summary>
        /// 客户端子系统名字
        /// </summary>
        [Required]
        public string AppName { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 客户端子系统唯一标识
        /// </summary>
        [Required]
        public string AppId { get; set; }

        /// <summary>
        /// 客户端子系统密钥
        /// </summary>
        [Required]
        public string AppSecret { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [DefaultValue(true)]
        public bool Available { get; set; }

        /// <summary>
        /// 是否是预置
        /// </summary>
        [DefaultValue(false)]
        public bool Preset { get; set; }

        public virtual ICollection<UserGroup> UserGroup { get; set; }
        public virtual ICollection<Control> Controls { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<Role> Roles { get; set; }

        public virtual ICollection<UserInfo> UserInfo { get; set; }
    }
}