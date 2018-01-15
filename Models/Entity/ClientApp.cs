using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("ClientApp")]
    public partial class ClientApp : BaseEntity
    {
        public ClientApp()
        {
            //UserGroup = new HashSet<UserGroup>();
            UserInfo = new HashSet<UserInfo>();
        }


        [Required]
        public string AppName { get; set; }

        [Required]
        public string AppId { get; set; }

        [Required]
        public string AppSecret { get; set; }

        public virtual ICollection<UserGroup> UserGroup { get; set; }

        public virtual ICollection<UserInfo> UserInfo { get; set; }
    }
}
