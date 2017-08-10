using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("Permission")]
    public class Permission
    {
        public Permission()
        {
            Function = new HashSet<Function>();
            UserPermission = new HashSet<UserPermission>();
            Role = new HashSet<Role>();
        }

        [Key]
        public int Id { get; set; }

        [Display(Name = "х╗оч")]
        [Required]
        public string PermissionName { get; set; }

        public string Description { get; set; }


        public virtual ICollection<Function> Function { get; set; }


        public virtual ICollection<UserPermission> UserPermission { get; set; }


        public virtual ICollection<Role> Role { get; set; }
    }
}