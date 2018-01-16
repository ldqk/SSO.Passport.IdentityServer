using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("Role")]
    public partial class Role : BaseEntity
    {
        public Role()
        {
            UserGroupPermission = new HashSet<UserGroupPermission>();
            Permission = new HashSet<Permission>();
            UserInfo = new HashSet<UserInfo>();
        }

        /// <summary>
        /// ½ÇÉ«Ãû
        /// </summary>
        [Required]
        public string RoleName { get; set; }

        /// <summary>
        /// ½ÇÉ«ÃèÊö
        /// </summary>
        public string Description { get; set; }

        public int ClientAppId { get; set; }

        public virtual ClientApp ClientApp { get; set; }

        public virtual ICollection<UserGroupPermission> UserGroupPermission { get; set; }

        public virtual ICollection<Permission> Permission { get; set; }

        public virtual ICollection<UserInfo> UserInfo { get; set; }
    }
}