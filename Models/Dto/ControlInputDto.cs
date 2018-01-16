using System.ComponentModel.DataAnnotations;
using Models.Entity;
using Models.Enum;

namespace Models.Dto
{
    public partial class ControlInputDto : BaseEntity
    {

        [Display(Name = "功能名称")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "控制器名称")]
        [Required]
        public string Controller { get; set; }

        [Display(Name = "方法名称")]
        [Required]
        public string Action { get; set; }

        [Display(Name = "HTTP请求方式")]
        [Required]
        public HttpMethod HttpMethod { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [Required]
        public bool IsAvailable { get; set; }
        public int ClientAppId { get; set; }

    }
}