using System.Configuration;
using System.Text;
using System.Web;
using Masuit.Tools.NoSQL;
using Newtonsoft.Json;
using SSO.Core.Model;

namespace SSO.Core.Client
{
    public class LoginService
    {
        public static RedisHelper RedisHelper { get; set; } = new RedisHelper();
        /// <summary>
        /// 根据ticket来解密获取当前用户信息
        /// </summary>
        /// <param name="token">即用户的主键id</param>
        /// <returns></returns>
        public static UserInfoLoginModel GetUserInfo(string token)
        {
            if (RedisHelper.KeyExists(token))
            {
                string userid = RedisHelper.GetString(token);
                string data = AuthernUtil.CallServerApi($"/Api/User/{userid}");
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

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public static string Logout(string returnUrl)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("<script type='text/javascript'>");
            sbScript.AppendFormat($"window.location.href='{ConfigurationManager.AppSettings["PassportUrl"] ?? $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}"}/Passport/Logout?returnUrl={returnUrl}';");
            sbScript.Append("</script>");
            return sbScript.ToString();
        }

        /// <summary>
        /// 获取用户信息、访问控制权限、菜单
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static UserModel GetAccessControls(string appid, string token)
        {
            string data = AuthernUtil.CallServerApi($"/Api/User/{appid}/{token}");
            return JsonConvert.DeserializeObject<UserModel>(data);
        }
    }
}