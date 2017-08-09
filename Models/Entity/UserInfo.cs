using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("UserInfo")]
    public class UserInfo
    {
        public UserInfo()
        {
            UserPermission = new HashSet<UserPermission>();
            Role = new HashSet<Role>();
            UserGroup = new HashSet<UserGroup>();
            Id = Guid.NewGuid();
            LastLoginTime = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }

        [Display(Name = "用户名"), Required(ErrorMessage = "用户名不能为空！")]
        public string Username { get; set; }

        [Display(Name = "密码"), Required(ErrorMessage = "登录密码不能为空！")]
        public string Password { get; set; }

        [Required]
        public string SaltKey { get; set; }

        [Display(Name = "手机号码"), Required(ErrorMessage = "手机号码不能为空！")]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        [Display(Name = "电子邮箱"), Required(ErrorMessage = "邮箱地址不能为空！")]
        public string Email { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public virtual ICollection<UserPermission> UserPermission { get; set; }

        public virtual ICollection<Role> Role { get; set; }

        public virtual ICollection<UserGroup> UserGroup { get; set; }
    }
}