using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("UserGroupPermission")]
    public partial class UserGroupPermission : BaseEntity
    {
        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool HasPermission { get; set; }

        [ForeignKey("UserGroup")]
        public int UserGroupId { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public virtual UserGroup UserGroup { get; set; }
    }
}
