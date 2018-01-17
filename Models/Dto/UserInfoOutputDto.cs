using System;
using System.Collections.Generic;

namespace Models.Dto
{
    public partial class UserInfoOutputDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 密码加密盐
        /// </summary>
        public string SaltKey { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 是否是预置账户
        /// </summary>
        public bool IsPreset { get; set; }

        /// <summary>
        /// 是否是内置管理员
        /// </summary>
        public bool IsMaster { get; set; }

        public virtual ICollection<ClientAppOutputDto> ClientApp { get; set; }


        public virtual ICollection<UserPermissionOutputDto> UserPermission { get; set; }

        public virtual ICollection<RoleOutputDto> Role { get; set; }

        public virtual ICollection<UserGroupOutputDto> UserGroup { get; set; }
        public virtual ICollection<LoginRecordDto> LoginRecords { get; set; }
    }
}