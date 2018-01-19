using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using AutoMapper;
using IBLL;
using Masuit.Tools.NoSQL;
using Masuit.Tools.Security;
using Models.Dto;
using Models.Entity;
using Newtonsoft.Json;

namespace SSO.Passport.IdentityServer.Controllers
{
    /// <summary>
    /// 公共api
    /// </summary>
    [System.Web.Http.Route("api/{action}")]
    public class PublicController : ApiController
    {
        public IUserInfoBll UserInfoBll { get; set; }
        public static RedisHelper RedisHelper { get; set; } = new RedisHelper();
        public PublicController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }
        protected IHttpActionResult ResultData(object data, bool isTrue = true, string message = "", bool isLogin = true)
        {
            return Json(new { IsLogin = isLogin, Success = isTrue, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }, Encoding.UTF8);
        }
        /// <summary>
        /// 获取访问控制权限
        /// </summary>
        /// <param name="token"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost, System.Web.Http.HttpGet, System.Web.Http.Route("api/acl/{appid}/{token}")]
        public List<ControlOutputDto> AccessControls(string token, string appid)
        {
            if (RedisHelper.KeyExists(token))
            {
                RedisHelper.Expire(token, TimeSpan.FromMinutes(20));
                UserInfoDto user = RedisHelper.GetString<UserInfoDto>(token);
                List<ControlOutputDto> controls = UserInfoBll.GetAccessControls(appid, user.Id);
                return controls;
            }
            return new List<ControlOutputDto>();
        }

        /// <summary>
        /// 获取菜单权限
        /// </summary>
        /// <param name="token"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost, System.Web.Http.HttpGet, System.Web.Http.Route("api/menus/{appid}/{token}")]
        public List<MenuOutputDto> Menus(string token, string appid)
        {
            if (RedisHelper.KeyExists(token))
            {
                RedisHelper.Expire(token, TimeSpan.FromMinutes(20));
                UserInfoDto user = RedisHelper.GetString<UserInfoDto>(token);
                List<MenuOutputDto> menus = UserInfoBll.GetMenus(appid, user.Id);
                return menus;
            }
            return new List<MenuOutputDto>();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost, System.Web.Http.HttpGet, System.Web.Http.Route("api/user/{token}")]
        public UserInfoDto User(string token)
        {
            if (RedisHelper.KeyExists(token))
            {
                RedisHelper.Expire(token, TimeSpan.FromMinutes(20));
                UserInfoDto user = RedisHelper.GetString<UserInfoDto>(token);
                return user;
            }
            return null;
        }

        /// <summary>
        /// 接口方式登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public string Login(string username, string password)
        {
            UserInfoDto user = UserInfoBll.Login(username, password);
            if (user != null)
            {
                string token = Guid.NewGuid().ToString().MDString();
                bool b = RedisHelper.SetString(token, user, TimeSpan.FromMinutes(20));
                if (b)
                {
                    return token;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查登录状态
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost, System.Web.Http.Route("api/check/{token}")]
        public bool CheckLogin(string token)
        {
            if (RedisHelper.KeyExists(token))
            {
                RedisHelper.Expire(token, TimeSpan.FromMinutes(20));
                return true;
            }
            return false;
        }

        /// <summary>
        /// api注销登陆
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet, System.Web.Http.HttpPost, System.Web.Http.Route("api/logout/{token}")]
        public bool Logout(string token)
        {
            return RedisHelper.DeleteKey(token);
        }

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
            if (RedisHelper.KeyExists(token))
            {
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
                if (regex.Match(password).Success)
                {
                    UserInfoDto user = RedisHelper.GetString<UserInfoDto>(token);
                    bool b = UserInfoBll.ChangePassword(user.Id, old, password);
                    return ResultData(null, b, b ? $"密码修改成功，新密码为：{password}！" : "密码修改失败，可能是原密码不正确！");
                }

                return ResultData(null, false, "密码强度值不够，密码必须包含数字，必须包含小写或大写字母，必须包含至少一个特殊符号，至少6个字符，最多30个字符！");
            }
            return ResultData(null, false, "用户未登录系统！");
        }

        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="token"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public IHttpActionResult ChangeUsername(string token, string username)
        {
            if (RedisHelper.KeyExists(token))
            {
                UserInfoDto user = RedisHelper.GetString<UserInfoDto>(token);
                UserInfo userInfo = UserInfoBll.GetById(user.Id);
                if (!username.Equals(userInfo.Username) && UserInfoBll.UsernameExist(username))
                {
                    return ResultData(null, false, $"用户名{username}已经存在，请尝试更换其他用户名！");
                }

                userInfo.Username = username;
                bool b = UserInfoBll.UpdateEntitySaved(userInfo);
                return ResultData(Mapper.Map<UserInfoDto>(userInfo), b, b ? $"用户名修改成功，新用户名为{username}。" : "用户名修改失败！");
            }
            return ResultData(null, false, "用户未登录系统！");
        }
    }
}