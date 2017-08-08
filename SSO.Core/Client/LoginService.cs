using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Security;
using Newtonsoft.Json;

namespace SSO.Core.Client
{
    public class LoginService
    {
        /// <summary>
        /// 根据ticket来解密获取当前用户信息
        /// </summary>
        /// <param name="ticket">即用户的主键id</param>
        /// <returns></returns>
        public static UserInfoLoginModel GetUserInfo(string ticket)
        {
            //todo 解密ticket 获取用户Id
            string data = AuthernUtil.CallServerApi("/Api/GetUser", new Dictionary<string, string> { { "id", ticket } });
            try
            {
                return JsonConvert.DeserializeObject<UserInfoLoginModel>(data);
            }
            catch (Exception e)
            {
                return null;
            }
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