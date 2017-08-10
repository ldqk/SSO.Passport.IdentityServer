using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Enum;

namespace Models.Entity
{
    [Table("Function")]
    public class Function
    {
        public Function()
        {
            IsAvailable = true;
        }
        [Key]
        public int Id { get; set; }

        [Display(Name = "功能名称")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "控制器名称")]
        [Required]
        public string Controller { get; set; }

        [Display(Name = "方法名称")]
        [Required]
        public string Action { get; set; }

        [Display(Name = "图标URL地址")]
        public string IconUrl { get; set; }

        [Display(Name = "class样式")]
        public string CssStyle { get; set; }

        [Display(Name = "HTTP请求方式")]
        [Required]
        public HttpMethod HttpMethod { get; set; }

        public bool IsAvailable { get; set; }

        public int ParentId { get; set; }

        [Display(Name = "所属权限")]
        [ForeignKey("Permission")]
        public int PermissionId { get; set; }

        [Display(Name = "权限类型")]
        public FunctionType FunctionType { get; set; }

        public virtual Permission Permission { get; set; }
    }
}