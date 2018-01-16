using System;
using Models.Entity;

namespace Models.Dto
{
    public class LoginRecordDto : BaseEntity
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
    }
}