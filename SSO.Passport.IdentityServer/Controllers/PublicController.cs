using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using AutoMapper;
using Common;
using IBLL;
using Masuit.Tools;
using Masuit.Tools.NoSQL;
using Masuit.Tools.Security;
using Models.Dto;
using Models.Entity;
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
        public IClientAppBll ClientAppBll { get; set; }
        public PublicController(IUserInfoBll userInfoBll, ILoginRecordBll loginRecordBll, IClientAppBll clientAppBll)
        {
            UserInfoBll = userInfoBll;
            LoginRecordBll = loginRecordBll;
            ClientAppBll = clientAppBll;
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
        /// <param name="token">token或用户id</param>
        /// <returns></returns>
        [HttpPost, HttpGet, Route("api/user/{appid}/{token}")]
        public object User(string appid, string token)
        {
            UserInfoDto user;
            if (Guid.TryParse(token, out Guid userid))
            {
                user = UserInfoBll.GetById(userid).Mapper<UserInfoDto>();
            }
            else
            {
                if (!RedisHelper.KeyExists(token)) return null;
                RedisHelper.Expire(token, TimeSpan.FromMinutes(20));
                user = RedisHelper.GetString<UserInfoDto>(token);
            }
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
        /// <param name="token">token或用户id</param>
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
        /// <param name="token">token或用户id</param>
        /// <returns></returns>
        [HttpGet, HttpPost, Route("api/logout/{token}")]
        public bool Logout(string token) => RedisHelper.DeleteKey(token);

        /// <summary>
        /// api修改密码
        /// </summary>
        /// <param name="token">登录token或用户id</param>
        /// <param name="old">旧密码</param>
        /// <param name="password">新密码</param>
        /// <param name="confirm">确认新密码</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ChangePasspord(string token, string old, string password, string confirm)
        {
            UserInfoDto user;
            if (Guid.TryParse(token, out var userid))
            {
                //session登录
                user = UserInfoBll.GetById(userid).Mapper<UserInfoDto>();
            }
            else
            {
                //api登录
                if (!RedisHelper.KeyExists(token)) return ResultData(null, false, "用户未登录系统！");
                user = RedisHelper.GetString<UserInfoDto>(token);
            }
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
            bool b = UserInfoBll.ChangePassword(user.Id, old, password);
            return ResultData(null, b, b ? $"密码修改成功，新密码为：{password}！" : "密码修改失败，可能是原密码不正确！");
        }

        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="token">token或用户id</param>
        /// <param name="username"></param>
        /// <returns></returns>
        public IHttpActionResult ChangeUsername(string token, string username)
        {
            UserInfoDto user;
            if (Guid.TryParse(token, out var userid))
            {
                //session登录
                user = UserInfoBll.GetById(userid).Mapper<UserInfoDto>();
            }
            else
            {
                //api登录
                if (!RedisHelper.KeyExists(token)) return ResultData(null, false, "用户未登录系统！");
                user = RedisHelper.GetString<UserInfoDto>(token);
            }
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
        /// <param name="token">token或用户id</param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IHttpActionResult LoginRecord(string token, int page = 1, int size = 10)
        {
            UserInfoDto user;
            if (Guid.TryParse(token, out var userid))
            {
                //session登录
                user = UserInfoBll.GetById(userid).Mapper<UserInfoDto>();
            }
            else
            {
                //api登录
                if (!RedisHelper.KeyExists(token)) return ResultData(null, false, "用户未登录系统！");
                user = RedisHelper.GetString<UserInfoDto>(token);
            }
            List<LoginRecordDto> list = LoginRecordBll.LoadPageEntitiesNoTracking<DateTime, LoginRecordDto>(page, size, out int total, r => r.UserInfoId.Equals(user.Id), r => r.LoginTime, false).ToList();
            int pages = (int)Math.Ceiling(total * 1.0 / size);
            return ResultData(new { list, pages });
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="pwd"></param>
        /// <param name="confirm"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Register(string appid, string name, string email, string pwd, string confirm)
        {
            if (!ClientAppBll.Any(a => a.AppId.Equals(appid)))
            {
                return ResultData(null, false, "应用不存在！");
            }
            if (String.IsNullOrEmpty(name.Trim()))
            {
                return ResultData(null, false, "用户名不能为空");
            }

            if (!email.MatchEmail())
            {
                return ResultData(null, false, "邮箱格式不正确！");
            }

            if (pwd.Length <= 6)
            {
                return ResultData(null, false, "密码过短，至少需要6个字符！");
            }

            if (!pwd.Equals(confirm))
            {
                return ResultData(null, false, "两次输入的密码不一致！");
            }

            var regex = new Regex(@"(?=.*[0-9])                     #必须包含数字
                                            (?=.*[a-zA-Z])                  #必须包含小写或大写字母
                                            (?=([\x21-\x7e]+)[^a-zA-Z0-9])  #必须包含特殊符号
                                            .{6,30}                         #至少6个字符，最多30个字符
                                            ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            if (regex.Match(pwd).Success)
            {
                UserInfoDto user = UserInfoBll.Register(new UserInfo() { Username = name, Password = pwd, Email = email });
                if (user != null)
                {
                    ClientApp app = ClientAppBll.GetFirstEntity(a => a.AppId.Equals(appid));
                    if (app.Available)
                    {
                        app.UserInfo.Add(UserInfoBll.GetById(user.Id));
                        bool b = ClientAppBll.UpdateEntitySaved(app);
                        return ResultData(user, true, b ? "用户注册成功！" : "用户注册成功，但尚未分配到指定的应用子系统，请联系管理员！");
                    }
                    return ResultData(user, false, $"用户注册成功，但由于【{app.AppName}】网站当前服务不可用，而无法登陆，请联系管理员！");
                }

                return ResultData(null, false, "用户注册失败！");
            }

            return ResultData(null, false, "密码强度值不够，密码必须包含数字，必须包含小写或大写字母，必须包含至少一个特殊符号，至少6个字符，最多30个字符！");
        }

        /// <summary>
        /// 根据id获取用户，仅限服务端本机
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("api/user/{id}")]
        public UserInfoDto GetUser(Guid id) => UserInfoBll.GetById(id).Mapper<UserInfoDto>();
    }
}