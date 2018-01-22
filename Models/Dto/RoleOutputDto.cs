using System.Collections.Generic;
using Models.Entity;

namespace Models.Dto
{
    public partial class RoleOutputDto : BaseEntity
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public bool HasRole { get; set; }

        public virtual ICollection<PermissionOutputDto> Permission { get; set; }
    }
}