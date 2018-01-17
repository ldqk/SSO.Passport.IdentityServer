using System.Collections.Generic;
using Models.Entity;

namespace Models.Dto
{
    public partial class PermissionOutputDto : BaseEntity
    {
        /// <summary>
        /// 权限名
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; set; }

        public int? ParentId { get; set; }

        public virtual ICollection<ControlOutputDto> Controls { get; set; }

        public virtual ICollection<MenuOutputDto> Menu { get; set; }

    }
}