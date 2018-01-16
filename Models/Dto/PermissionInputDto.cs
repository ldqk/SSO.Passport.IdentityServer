using System.ComponentModel.DataAnnotations;
using Models.Entity;

namespace Models.Dto
{
    public partial class PermissionInputDto : BaseEntity
    {
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

    }
}