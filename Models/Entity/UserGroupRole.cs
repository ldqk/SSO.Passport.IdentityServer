using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("UserGroupRole")]
    public partial class UserGroupRole : BaseEntity
    {
        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool HasRole { get; set; }

        [ForeignKey("UserGroup")]
        public int UserGroupId { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public virtual UserGroup UserGroup { get; set; }
    }
}
