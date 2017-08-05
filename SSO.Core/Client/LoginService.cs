using System;
using System.Configuration;
using System.Net.Http;
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
            HttpClient client = new HttpClient() { BaseAddress = new Uri(ConfigurationManager.AppSettings["PassportUrl"] ?? $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}") };
            int time = DateTime.Now.GetTotalSeconds().ToInt32();
            string salt = ConfigurationManager.AppSettings["encryptSalt"] ?? "masuit".DesEncrypt();
            string hash = (time + salt).MDString();
            Task<string> task = client.GetStringAsync($"/Api/GetUser/{ticket}?time={time}&hash={hash}");
            try
            {
                string result = task.Result;
                return JsonConvert.DeserializeObject<UserInfoLoginModel>(result);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}