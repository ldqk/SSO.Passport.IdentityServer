using System.ComponentModel.DataAnnotations;
using Models.Entity;

namespace Models.Dto
{
    public partial class RoleInputDto : BaseEntity
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [Required]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }
        public int? ParentId { get; set; }
    }
}