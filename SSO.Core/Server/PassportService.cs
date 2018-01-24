using System;
using System.Text.RegularExpressions;
using Masuit.Tools;
using Masuit.Tools.NoSQL;
using Masuit.Tools.Security;
using SSO.Core.Client;

namespace SSO.Core.Server
{
    public class PassportService
    {
        public static RedisHelper RedisHelper { get; set; } = new RedisHelper();
        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        public static bool AuthernVertify(string token, DateTime timestamp) => AuthernUtil.CreateToken(timestamp) == token;

        public static string CreateTicket(string userId)
        {
            //加密userinfo
            string ticket = Guid.NewGuid().ToString().MDString();
            RedisHelper.SetString(ticket, userId, TimeSpan.FromMinutes(20));
            return ticket;
        }

        public static string GetReturnUrl(string userId, string token, string returnUrl)
        {
            return $"{returnUrl.Replace(new Regex("ticket=(.{0,36})&token=(.{0,32})"), String.Empty)}{(returnUrl.Contains("?") ? "&" : "?")}ticket={CreateTicket(userId)}&token={token}".Replace(new Regex(@"(\?&+)"), "?").Replace(new Regex(@"&+"), "&");
        }
    }
}