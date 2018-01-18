using Models.Entity;

namespace Models.Dto
{
    public partial class MenuOutputDto : BaseEntity
    {
        /// <summary>
        /// 菜单名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 前端路由，为angular、vue等提供
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// 前端路由名，为angular、vue等提供
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// 菜单图标地址
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// css的class样式
        /// </summary>
        public string CssStyle { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// 上级菜单
        /// </summary>
        public int? ParentId { get; set; }

        public int ClientAppId { get; set; }

    }
}