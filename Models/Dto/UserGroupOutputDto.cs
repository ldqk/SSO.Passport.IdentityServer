using System.Collections.Generic;
using Models.Entity;

namespace Models.Dto
{
    public partial class UserGroupOutputDto : BaseEntity
    {
        /// <summary>
        /// 用户组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 父级组
        /// </summary>
        public int? ParentId { get; set; }

        public virtual ICollection<UserGroupPermissionOutputDto> UserGroupPermission { get; set; }
    }
}