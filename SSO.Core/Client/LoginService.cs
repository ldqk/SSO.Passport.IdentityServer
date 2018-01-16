using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using Masuit.Tools.NoSQL;
using Newtonsoft.Json;

namespace SSO.Core.Client
{
    public class LoginService
    {
        public static RedisHelper RedisHelper { get; set; } = new RedisHelper();
        /// <summary>
        /// 根据ticket来解密获取当前用户信息
        /// </summary>
        /// <param name="ticket">即用户的主键id</param>
        /// <returns></returns>
        public static UserInfoLoginModel GetUserInfo(string ticket)
        {
            //todo 解密ticket 获取用户Id
            if (RedisHelper.KeyExists(ticket))
            {
                string userid = RedisHelper.GetString(ticket);
                string data = AuthernUtil.CallServerApi("/Api/GetUser", new Dictionary<string, string> { { "id", userid } });
                try
                {
                    return JsonConvert.DeserializeObject<UserInfoLoginModel>(data);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public static string Logout(string returnUrl)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("<script type='text/javascript'>");
            sbScript.AppendFormat($"window.location.href='{ConfigurationManager.AppSettings["PassportUrl"] ?? $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}"}/Passport/Logout?returnUrl={returnUrl}';");
            sbScript.Append("</script>");
            return sbScript.ToString();
        }
    }
}