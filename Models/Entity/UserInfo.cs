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

        [Required(ErrorMessage = "用户名不能为空！")]
        public string Username { get; set; }

        [Required(ErrorMessage = "登录密码不能为空！")]
        public string Password { get; set; }

        [Required]
        public string SaltKey { get; set; }

        [Required(ErrorMessage = "手机号码不能为空！")]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "邮箱地址不能为空！")]
        public string Email { get; set; }

        public DateTime? LastLoginTime { get; set; }


        public virtual ICollection<UserPermission> UserPermission { get; set; }


        public virtual ICollection<Role> Role { get; set; }


        public virtual ICollection<UserGroup> UserGroup { get; set; }
    }
}