using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Enum;

namespace Models.Entity
{
    [Table("Control")]
    public partial class Control : BaseEntity
    {
        public Control()
        {
            Permission = new HashSet<Permission>();
            IsAvailable = true;
        }

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
        [Display(Name = "请求路径")]
        public string Path { get; set; }

        [Display(Name = "HTTP请求方式")]
        [Required]
        public HttpMethod HttpMethod { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [Required, DefaultValue(true)]
        public bool IsAvailable { get; set; }

        public int ClientAppId { get; set; }

        public virtual ClientApp ClientApp { get; set; }

        public virtual ICollection<Permission> Permission { get; set; }
    }
}