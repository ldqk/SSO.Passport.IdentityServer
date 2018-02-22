using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Net;
using Masuit.Tools.NoSQL;
using Masuit.Tools.Security;
using SSO.Core.Model;

namespace SSO.Core.Client
{
    public class AuthernUtil
    {
        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string CreateToken(DateTime timestamp) => timestamp.GetTotalMilliseconds().ToString().MDString2();

        public static RedisHelper RedisHelper { get; set; } = new RedisHelper();
        /// <summary>
        /// 获取当前客户端登录用户
        /// </summary>
        public static UserInfoLoginModel CurrentUser
        {
            get
            {
                UserInfoLoginModel user = HttpContext.Current.Session?.GetByCookieRedis<UserInfoLoginModel>(Constants.USER_SESSION_KEY);
                if (user is null)
                {
                    string token = HttpContext.Current.Request.Headers["Authorization"] ?? HttpContext.Current.Request["token"];
                    if (RedisHelper.KeyExists(token))
                    {
                        user = RedisHelper.GetString<UserInfoLoginModel>(token);
                        RedisHelper.Expire(token, TimeSpan.FromMinutes(20));
                    }
                }
                return user;
            }
        }

        /// <summary>
        /// 注销客户端当前用户
        /// </summary>
        public static bool Logout() => HttpContext.Current.Session.RemoveByCookieRedis(Constants.USER_SESSION_KEY);

        /// <summary>
        /// 调用服务器端接口数据<br/> 
        /// </summary>
        /// <param name="path">API路径，格式：/Controller/Action</param>
        /// <param name="param">参数，键值对</param>
        /// <returns></returns>
        public static string CallServerApi(string path, IDictionary<string, string> param = null)
        {
            using (var client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["PassportUrl"] ?? $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}") })
            {
                int time = DateTime.Now.GetTotalSeconds().ToInt32();
                string salt = ConfigurationManager.AppSettings["encryptSalt"] ?? "masuit".DesEncrypt();
                string hash = (time + salt).MDString();
                StringBuilder sb = new StringBuilder();
                param?.ForEach(kv => sb.Append(kv.Key + "=" + kv.Value + "&"));
                var res = client.GetAsync($"{path}?{sb}time={time}&hash={hash}").Result;
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    return res.Content.ReadAsStringAsync().Result;
                }
                return $"请求授权中心路径{path}时服务器响应异常！";
            }
        }

        /// <summary>
        /// 获取SSO登陆地址
        /// </summary>
        /// <param name="token"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string GetAuthorityUrl(string token, DateTime timestamp) => $"{ConfigurationManager.AppSettings["PassportUrl"]?.TrimEnd('/')}/Passport/PassportCenter?token={token}&timestamp={timestamp}";
    }

    /// <summary>
    /// 静态变量帮助类
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// 用户SessionKey
        /// </summary>
        public const string USER_SESSION_KEY = "UserSessionKey";

        /// <summary>
        /// 用户CookieKey
        /// </summary>
        public const string USER_COOKIE_KEY = "UserCookieKey";

        public const string TOKEN_KEY = "TokenKey";
    }
}
