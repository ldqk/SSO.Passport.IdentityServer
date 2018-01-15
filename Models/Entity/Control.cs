using System.Collections.Generic;
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
        }


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

        [Required]
        public bool IsAvailable { get; set; }

        public virtual ICollection<Permission> Permission { get; set; }
    }
}
