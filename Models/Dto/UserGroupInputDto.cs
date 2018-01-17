using System.ComponentModel.DataAnnotations;
using Models.Entity;

namespace Models.Dto
{
    public partial class UserGroupInputDto : BaseEntity
    {
        /// <summary>
        /// 用户名名
        /// </summary>
        [Required, Display(Name = "用户组")]
        public string GroupName { get; set; }

        /// <summary>
        /// 父级组
        /// </summary>
        [Display(Name = "父级id")]
        public int? ParentId { get; set; }
    }
}