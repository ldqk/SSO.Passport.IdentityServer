using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("UserGroup")]
    public partial class UserGroup : BaseEntity
    {
        public UserGroup()
        {
            UserGroupRole = new HashSet<UserGroupRole>();
            UserInfo = new HashSet<UserInfo>();
        }

        [Required, Display(Name = "用户组")]
        public string GroupName { get; set; }

        [Display(Name = "父级id")]
        public int? ParentId { get; set; }

        public virtual ICollection<UserGroup> Children { get; set; }

        public virtual UserGroup Parent { get; set; }
        public virtual ICollection<ClientApp> ClientApp { get; set; }

        public virtual ICollection<UserGroupRole> UserGroupRole { get; set; }

        public virtual ICollection<UserInfo> UserInfo { get; set; }
    }
}
