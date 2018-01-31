using System;

namespace SSO.Core.Model
{
    public class TokenModel
    {
        /// <summary>
        /// 唯一Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}