using System.ComponentModel.DataAnnotations;

namespace Models.Entity
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

    }
}