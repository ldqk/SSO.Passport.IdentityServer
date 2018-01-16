using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Models.Entity;

namespace Models.Dto
{
    public partial class MenuInputDto : BaseEntity
    {

        [Display(Name = "菜单名称")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "菜单路由")]
        [Required, DefaultValue("/")]
        public string Url { get; set; }

        [Display(Name = "图标URL地址")]
        public string IconUrl { get; set; }

        [Display(Name = "class样式")]
        public string CssStyle { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [Required]
        public string IsAvailable { get; set; }

        /// <summary>
        /// 父级菜单
        /// </summary>
        public int? ParentId { get; set; }
        public int ClientAppId { get; set; }

    }
}