using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("Menu")]
    public partial class Menu : BaseEntity
    {
        public Menu()
        {
            Children = new HashSet<Menu>();
            Permission = new HashSet<Permission>();
            IsAvailable = true;
        }

        /// <summary>
        /// 菜单名
        /// </summary>
        [Display(Name = "菜单名称")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 菜单URL
        /// </summary>
        [Display(Name = "菜单Url")]
        [DefaultValue("/")]
        public string Url { get; set; }

        /// <summary>
        /// 前端路由，为angular、vue等提供
        /// </summary>
        [Display(Name = "前端路由，为angular、vue等提供")]
        public string Route { get; set; }

        /// <summary>
        /// 前端路由名，为angular、vue等提供
        /// </summary>
        [Display(Name = "前端路由名，为angular、vue等提供")]
        public string RouteName { get; set; }

        /// <summary>
        /// 图标URL地址
        /// </summary>
        [Display(Name = "图标URL地址")]
        public string IconUrl { get; set; }

        /// <summary>
        /// class样式
        /// </summary>
        [Display(Name = "class样式")]
        public string CssStyle { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [Required]
        public bool IsAvailable { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 父级菜单
        /// </summary>
        public int? ParentId { get; set; }

        public int ClientAppId { get; set; }

        public virtual ClientApp ClientApp { get; set; }

        public virtual ICollection<Menu> Children { get; set; }

        public virtual Menu Parent { get; set; }

        public virtual ICollection<Permission> Permission { get; set; }
    }
}