using System.ComponentModel.DataAnnotations;
using Models.Entity;
using Models.Enum;

namespace Models.Dto
{
    public partial class ControlInputDto : BaseEntity
    {
        /// <summary>
        /// 访问控制名
        /// </summary>
        [Display(Name = "功能名称")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 控制器名
        /// </summary>
        [Display(Name = "控制器名称")]
        public string Controller { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        [Display(Name = "方法名称")]
        public string Action { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// http请求方式
        /// </summary>
        [Display(Name = "HTTP请求方式")]
        [Required]
        public HttpMethod HttpMethod { get; set; }

        public int ClientAppId { get; set; }
    }
}