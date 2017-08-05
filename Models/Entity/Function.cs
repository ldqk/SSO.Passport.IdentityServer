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

        [Required]
        public string Controller { get; set; }

        [Required]
        public string Action { get; set; }

        public string IconUrl { get; set; }

        public string CssStyle { get; set; }

        [Required]
        public string HttpMethod { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        public int ParentId { get; set; }

        [ForeignKey("Permission")]
        public int PermissionId { get; set; }

        public FunctionType FunctionType { get; set; }

        public virtual Permission Permission { get; set; }
    }
}