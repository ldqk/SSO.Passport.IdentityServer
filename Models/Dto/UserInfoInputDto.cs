using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.Dto
{
    public partial class UserInfoInputDto
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名"), Required(ErrorMessage = "用户名不能为空！")]
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码"), Required(ErrorMessage = "登录密码不能为空！")]
        public string Password { get; set; }
        /// <summary>
        /// 密码加密盐
        /// </summary>
        [Required]
        public string SaltKey { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "电子邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 是否是预置账户
        /// </summary>
        [DefaultValue(false)]
        public bool IsPreset { get; set; }

        /// <summary>
        /// 是否是内置管理员
        /// </summary>
        [DefaultValue(false)]
        public bool IsMaster { get; set; }
    }
}