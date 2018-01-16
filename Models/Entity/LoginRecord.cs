using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    [Table("LoginRecord")]
    public class LoginRecord : BaseEntity
    {
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 登录所在省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 详细物理地址
        /// </summary>
        public string PhysicAddress { get; set; }

        [ForeignKey("UserInfo")]
        public Guid UserInfoId { get; set; }

        public virtual UserInfo UserInfo { get; set; }
    }
}