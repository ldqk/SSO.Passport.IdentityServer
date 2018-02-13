using System;
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
        /// <param name="token">token或用户id</param>
        /// <returns></returns>
        public static UserModel GetAccessControls(string token)
        {
            string appid = ConfigurationManager.AppSettings["AppId"];
            if (string.IsNullOrEmpty(appid))
            {
                throw new ArgumentException("appid未配置，请先在web.config中AppSettings节点下添加AppId配置信息");
            }
            if (RedisHelper.KeyExists(appid + token))
            {
                return RedisHelper.GetString<UserModel>(appid + token);
            }
            string data = AuthernUtil.CallServerApi($"/Api/User/{appid}/{token}");
            UserModel userModel = JsonConvert.DeserializeObject<UserModel>(data);
            RedisHelper.SetString(appid + token, userModel);
            return userModel;
        }

        /// <summary>
        /// api登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string Login(string name, string pwd)
        {
            string token = AuthernUtil.CallServerApi($"/Api/Login?username={name}&password={pwd}");
            if (!string.IsNullOrEmpty(token))
            {
                UserInfoLoginModel user = GetAccessControls(token).User;
                RedisHelper.SetString(token, user, TimeSpan.FromMinutes(20));
            }
            return token;
        }

        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="token">token或用户id</param>
        /// <param name="username"></param>
        public static string ChangeUsername(string token, string username)
        {
            return AuthernUtil.CallServerApi($"/Api/ChangeUsername?token={token}&username={username}");
        }

        /// <summary>
        /// 检查登录状态
        /// </summary>
        /// <param name="token">token或用户id</param>
        /// <returns></returns>
        public static string CheckLogin(string token)
        {
            return AuthernUtil.CallServerApi($"/Api/CheckLogin?token={token}");
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="token">token或用户id</param>
        /// <param name="old"></param>
        /// <param name="password"></param>
        /// <param name="confirm"></param>
        /// <returns></returns>
        public static string ChangePasspord(string token, string old, string password, string confirm)
        {
            return AuthernUtil.CallServerApi($"/Api/ChangePasspord?token={token}&old={old}&password={password}&confirm={confirm}");
        }

        /// <summary>
        /// 新用户注册
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="confirm"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string Register(string name, string email, string pwd, string confirm)
        {
            string appid = ConfigurationManager.AppSettings["AppId"];
            if (string.IsNullOrEmpty(appid))
            {
                throw new ArgumentException("appid未配置，请先在web.config中AppSettings节点下添加AppId配置信息");
            }
            return AuthernUtil.CallServerApi($"/Api/Register?appid={appid}&name={name}&email={email}&pwd={pwd}&confirm={confirm}");
        }

        /// <summary>
        /// 最近登陆记录
        /// </summary>
        /// <param name="token">token或用户id</param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string LoginRecord(string token, int page = 1, int size = 10)
        {
            return AuthernUtil.CallServerApi($"/Api/LoginRecord?token={token}&page={page}&size={size}");
        }
    }
}