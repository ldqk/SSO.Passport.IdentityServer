using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("FunctionType")]
    public class FunctionType
    {
        public FunctionType()
        {
            Function = new HashSet<Function>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string TypeName { get; set; }

        public string Description { get; set; }


        public virtual ICollection<Function> Function { get; set; }
    }
}