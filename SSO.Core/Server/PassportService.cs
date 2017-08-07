using System;
using System.Text.RegularExpressions;
using Masuit.Tools;
using SSO.Core.Client;

namespace SSO.Core.Server
{
    public class PassportService
    {
        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        public bool AuthernVertify(string token, DateTime timestamp) => AuthernUtil.CreateToken(timestamp) == token;

        public string CreateTicket(string userId)
        {
            //加密userinfo
            return userId;
        }

        public string GetReturnUrl(string userId, string token, string returnUrl)
        {
            return $"{returnUrl.Replace(new Regex("ticket=(.{0,36})&token=(.{0,32})"), String.Empty)}{(returnUrl.Contains("?") ? "&" : "?")}ticket={CreateTicket(userId)}&token={token}".Replace(new Regex(@"(\?&+)"), "?").Replace(new Regex(@"&+"), "&");
        }
    }
}