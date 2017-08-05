using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("Function")]
    public class Function
    {
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
        public string IsAvailable { get; set; }

        public string ParentId { get; set; }

        [ForeignKey("FunctionType")]
        public int PermissionId { get; set; }

        [ForeignKey("Permission")]
        public int FunctionTypeId { get; set; }

        public virtual FunctionType FunctionType { get; set; }

        public virtual Permission Permission { get; set; }
    }
}