using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using AutoMapper;
using IBLL;
using Masuit.Tools.NoSQL;
using Masuit.Tools.Security;
using Models.Dto;
using Newtonsoft.Json;

namespace SSO.Passport.IdentityServer.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// 公共api
    /// </summary>
    [Route("api/{action}")]
    public class PublicController : ApiController
    {
        public IUserInfoBll UserInfoBll { get; set; }
        public static RedisHelper RedisHelper { get; set; } = new RedisHelper();
        public ILoginRecordBll LoginRecordBll { get; set; }
        public PublicController(IUserInfoBll userInfoBll, ILoginRecordBll loginRecordBll)
        {
            UserInfoBll = userInfoBll;
            LoginRecordBll = loginRecordBll;
        }

        protected IHttpActionResult ResultData(object data, bool isTrue = true, string message = "", bool isLogin = true)
        {
            return Json(new { IsLogin = isLogin, Success = isTrue, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }, Encoding.UTF8);
        }

        /// <summary>
        /// 获取appid
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetAppid()
        {
            return ConfigurationManager.AppSettings["AppId"];
        }

        /// <summary>
        /// 获取用户信息、访问控制权限、菜单
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost, HttpGet, Route("api/user/{appid}/{token}")]
        public object User(string appid, string token)
        {
            if (!RedisHelper.KeyExists(token)) return null;
            RedisHelper.Expire(token, TimeSpan.FromMinutes(20));
            UserInfoDto user = RedisHelper.GetString<UserInfoDto>(token);
            List<ControlOutputDto> acl = UserInfoBll.GetAccessControls(appid, user.Id);
            List<MenuOutputDto> menus = UserInfoBll.GetMenus(appid, user.Id);
            return new { user, acl, menus };
        }

        /// <summary>
        /// 接口方式登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public string Login(string username, string password)
        {
            var user = UserInfoBll.Login(username, password);
            if (user == null) return string.Empty;
            string token = Guid.NewGuid().ToString().MDString();
            bool b = RedisHelper.SetString(token, user, TimeSpan.FromMinutes(20));
            return b ? token : string.Empty;
        }

        /// <summary>
        /// 检查登录状态
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet, HttpPost, Route("api/check/{token}")]
        public bool CheckLogin(string token)
        {
            if (!RedisHelper.KeyExists(token)) return false;
            RedisHelper.Expire(token, TimeSpan.FromMinutes(20));
            return true;
        }

        /// <summary>
        /// api注销登陆
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet, HttpPost, Route("api/logout/{token}")]
        public bool Logout(string token) => RedisHelper.DeleteKey(token);

        /// <summary>
        /// api修改密码
        /// </summary>
        /// <param name="token">登录token</param>
        /// <param name="old">旧密码</param>
        /// <param name="password">新密码</param>
        /// <param name="confirm">确认新密码</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ChangePasspord(string token, string old, string password, string confirm)
        {
            if (!RedisHelper.KeyExists(token)) return ResultData(null, false, "用户未登录系统！");
            if (password.Length <= 6)
            {
                return ResultData(null, false, "密码过短，至少需要6个字符！");
            }

            if (!password.Equals(confirm))
            {
                return ResultData(null, false, "两次输入的密码不一致！");
            }

            var regex = new Regex(@"(?=.*[0-9])                     #必须包含数字
                                            (?=.*[a-zA-Z])                  #必须包含小写或大写字母
                                            (?=([\x21-\x7e]+)[^a-zA-Z0-9])  #必须包含特殊符号
                                            .{6,30}                         #至少6个字符，最多30个字符
                                            ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            if (!regex.Match(password).Success) return ResultData(null, false, "密码强度值不够，密码必须包含数字，必须包含小写或大写字母，必须包含至少一个特殊符号，至少6个字符，最多30个字符！");
            UserInfoDto user = RedisHelper.GetString<UserInfoDto>(token);
            bool b = UserInfoBll.ChangePassword(user.Id, old, password);
            return ResultData(null, b, b ? $"密码修改成功，新密码为：{password}！" : "密码修改失败，可能是原密码不正确！");
        }

        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="token"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public IHttpActionResult ChangeUsername(string token, string username)
        {
            if (!RedisHelper.KeyExists(token)) return ResultData(null, false, "用户未登录系统！");
            var user = RedisHelper.GetString<UserInfoDto>(token);
            var userInfo = UserInfoBll.GetById(user.Id);
            if (!username.Equals(userInfo.Username) && UserInfoBll.UsernameExist(username))
            {
                return ResultData(null, false, $"用户名{username}已经存在，请尝试更换其他用户名！");
            }
            userInfo.Username = username;
            bool b = UserInfoBll.UpdateEntitySaved(userInfo);
            return ResultData(Mapper.Map<UserInfoDto>(userInfo), b, b ? $"用户名修改成功，新用户名为{username}。" : "用户名修改失败！");
        }

        /// <summary>
        /// 获取登陆记录
        /// </summary>
        /// <param name="token"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IHttpActionResult LoginRecord(string token, int page = 1, int size = 10)
        {
            if (!RedisHelper.KeyExists(token)) return ResultData(null, false, "用户未登录系统！");
            UserInfoDto user = RedisHelper.GetString<UserInfoDto>(token);
            List<LoginRecordDto> list = LoginRecordBll.LoadPageEntitiesNoTracking<DateTime, LoginRecordDto>(page, size, out int total, r => r.UserInfoId.Equals(user.Id), r => r.LoginTime, false).ToList();
            int pages = (int)Math.Ceiling(total * 1.0 / size);
            return ResultData(new { list, pages });
        }
    }
}