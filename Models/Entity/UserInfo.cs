using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("UserInfo")]
    public partial class UserInfo
    {
        public UserInfo()
        {
            Id = Guid.NewGuid();
            UserPermission = new HashSet<UserPermission>();
            Role = new HashSet<Role>();
            UserGroup = new HashSet<UserGroup>();
        }
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "用户名"), Required(ErrorMessage = "用户名不能为空！")]
        public string Username { get; set; }

        [Display(Name = "密码"), Required(ErrorMessage = "登录密码不能为空！")]
        public string Password { get; set; }

        [Required]
        public string SaltKey { get; set; }

        [StringLength(11)]
        [Display(Name = "手机号码"), Required(ErrorMessage = "手机号码不能为空！")]
        public string PhoneNumber { get; set; }

        [Display(Name = "电子邮箱"), Required(ErrorMessage = "邮箱地址不能为空！")]
        public string Email { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public int ClientAppId { get; set; }

        [ForeignKey("ClientAppId")]
        public virtual ClientApp ClientApp { get; set; }

        public virtual ICollection<UserPermission> UserPermission { get; set; }

        public virtual ICollection<Role> Role { get; set; }

        public virtual ICollection<UserGroup> UserGroup { get; set; }
    }
}
