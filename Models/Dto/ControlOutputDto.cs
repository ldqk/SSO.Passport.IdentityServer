using Models.Entity;

namespace Models.Dto
{
    public partial class ControlOutputDto : BaseEntity
    {
        /// <summary>
        /// 功能名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所在控制器
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// 请求到的方法名
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// http请求方式
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsAvailable { get; set; }

        public int ClientAppId { get; set; }

    }
}