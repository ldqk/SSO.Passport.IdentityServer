using System;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Common;
using IBLL;
using Masuit.Tools;
using Masuit.Tools.Net;
using Masuit.Tools.Security;
using Masuit.Tools.Strings;
using Models.Dto;
using Models.Entity;
using Newtonsoft.Json;
using SSO.Passport.IdentityServer.Models.Hangfire;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class PassportController : Controller
    {
        public IUserInfoBll UserInfoBll { get; set; }
        public ILoginRecordBll LoginRecordBll { get; set; }

        public PassportController(IUserInfoBll userInfoBll, ILoginRecordBll loginRecordBll)
        {
            UserInfoBll = userInfoBll;
            LoginRecordBll = loginRecordBll;
        }

        protected ActionResult ResultData(object data, bool isTrue = true, string message = "")
        {
            return Content(JsonConvert.SerializeObject(new { Success = isTrue, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }), "application/json");
        }

        /// <summary>
        /// 登录页
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            string from = Request["from"];
            if (!string.IsNullOrEmpty(from))
            {
                from = Server.UrlDecode(from);
                CookieHelper.SetCookie("refer", from);
            }

            if (Session.GetByCookieRedis<UserInfoOutputDto>() != null)
            {
                if (string.IsNullOrEmpty(from))
                {
                    return RedirectToAction("Index", "Home");
                }

                return Redirect(from);
            }

            if (Request.Cookies.Count > 2)
            {
                string name = CookieHelper.GetCookieValue("username");
                string pwd = CookieHelper.GetCookieValue("password")?.DesDecrypt(ConfigurationManager.AppSettings["BaiduAK"]);
                var userInfo = UserInfoBll.Login(name, pwd);
                if (userInfo != null)
                {
                    CookieHelper.SetCookie("username", name, DateTime.Now.AddDays(7));
                    CookieHelper.SetCookie("password", CookieHelper.GetCookieValue("password"), DateTime.Now.AddDays(7));
                    Session.SetByRedis(userInfo);
                    HangfireHelper.CreateJob(typeof(IHangfireBackJob), "LoginRecord", "default", userInfo, Request.UserHostAddress);
                    if (string.IsNullOrEmpty(from))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    return Redirect(from);
                }
            }

            return View();
        }

        /// <summary>
        /// 登录表单
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="valid">验证码</param>
        /// <param name="remem">记住密码</param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string valid, string remem)
        {
            string validSession = Session.GetByCookieRedis<string>("valid") ?? String.Empty; //将验证码从Session中取出来，用于登录验证比较
            if (String.IsNullOrEmpty(validSession) || !valid.Trim().Equals(validSession, StringComparison.InvariantCultureIgnoreCase))
            {
                return ResultData(null, false, "验证码错误");
            }

            Session.RemoveByCookieRedis("valid"); //验证成功就销毁验证码Session，非常重要
            if (String.IsNullOrEmpty(username.Trim()) || String.IsNullOrEmpty(password.Trim()))
            {
                return ResultData(null, false, "用户名或密码不能为空");
            }

            var userInfo = UserInfoBll.Login(username, password);
            if (userInfo != null)
            {
                Session.SetByRedis(userInfo);
                if (remem.Trim().Contains(new[] { "on", "true" })) //是否记住登录
                {
                    HttpCookie userCookie = new HttpCookie("username", Server.UrlEncode(username.Trim()));
                    Response.Cookies.Add(userCookie);
                    userCookie.Expires = DateTime.Now.AddDays(7);
                    HttpCookie passCookie = new HttpCookie("password", password.Trim().DesEncrypt(ConfigurationManager.AppSettings["BaiduAK"])) { Expires = DateTime.Now.AddDays(7) };
                    Response.Cookies.Add(passCookie);
                }

                HangfireHelper.CreateJob(typeof(IHangfireBackJob), "LoginRecord", "default", userInfo, Request.UserHostAddress);
                string refer = CookieHelper.GetCookieValue("refer");
                if (string.IsNullOrEmpty(refer))
                {
                    return ResultData(null, true, "/");
                }

                return ResultData(null, true, refer);
            }

            return ResultData(null, false, "用户名或密码错误");
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidateCode()
        {
            string code = Masuit.Tools.Strings.ValidateCode.CreateValidateCode(6);
            Session.SetByRedis(code, "valid"); //将验证码生成到Session中
            System.Web.HttpContext.Current.CreateValidateGraphic(code);
            Response.ContentType = "image/jpeg";
            return File(Encoding.UTF8.GetBytes(code), "image/jpeg");
        }

        /// <summary>
        /// 检查验证码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckValidateCode(string code)
        {
            string validSession = Session.GetByCookieRedis<string>("valid");
            if (String.IsNullOrEmpty(validSession) || !code.Trim().Equals(validSession, StringComparison.InvariantCultureIgnoreCase))
            {
                return ResultData(null, false, "验证码错误");
            }

            return ResultData(null, false, "验证码正确");
        }

        /// <summary>
        /// 获取当前登录的用户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserInfo()
        {
            UserInfoOutputDto user = Session.GetByCookieRedis<UserInfoOutputDto>();
#if DEBUG
            user = UserInfoBll.GetByUsername("masuit").Mapper<UserInfoOutputDto>();
#endif
            return ResultData(user);
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.RemoveByCookieRedis();
            CookieHelper.SetCookie("username", String.Empty, DateTime.Now.AddDays(-1));
            CookieHelper.SetCookie("password", String.Empty, DateTime.Now.AddDays(-1));
            if (Request.HttpMethod.ToLower().Equals("get"))
            {
                return RedirectToAction("Index", "Home");
            }

            return ResultData(null, message: "注销成功！");
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="pwd"></param>
        /// <param name="confirm"></param>
        /// <param name="valid"></param>
        /// <returns></returns>
        public ActionResult Register(string name, string email, string pwd, string confirm, string valid)
        {
            string validSession = Session.GetByCookieRedis<string>("valid") ?? String.Empty; //将验证码从Session中取出来，用于登录验证比较
            if (String.IsNullOrEmpty(validSession) || !valid.Trim().Equals(validSession, StringComparison.InvariantCultureIgnoreCase))
            {
                return ResultData(null, false, "验证码错误");
            }

            Session.RemoveByCookieRedis("valid"); //验证成功就销毁验证码Session，非常重要

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
                UserInfoOutputDto user = UserInfoBll.Register(new UserInfo() { Username = name, Password = pwd });
                if (user != null)
                {
                    return ResultData(user);
                }
                return ResultData(null, false, "用户注册失败！");
            }
            return ResultData(null, false, "密码强度值不够，密码必须包含数字，必须包含小写或大写字母，必须包含至少一个特殊符号，至少6个字符，最多30个字符！");
        }

        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public ActionResult ChangeUsername(int id, string username)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            if (!username.Equals(userInfo.Username) && UserInfoBll.UsernameExist(username))
            {
                return ResultData(null, false, $"用户名{username}已经存在，请尝试更换其他用户名！");
            }
            userInfo.Username = username;
            bool b = UserInfoBll.UpdateEntitySaved(userInfo);
            return ResultData(Mapper.Map<UserInfoOutputDto>(userInfo), b, b ? $"用户名修改成功，新用户名为{username}。" : "用户名修改失败！");
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="old"></param>
        /// <param name="pwd"></param>
        /// <param name="confirm"></param>
        /// <returns></returns>
        public ActionResult ChangePassword(Guid id, string old, string pwd, string confirm)
        {
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
                bool b = UserInfoBll.ChangePassword(id, old, pwd);
                return ResultData(null, b, b ? $"密码修改成功，新密码为：{pwd}！" : "密码修改失败，可能是原密码不正确！");
            }
            return ResultData(null, false, "密码强度值不够，密码必须包含数字，必须包含小写或大写字母，必须包含至少一个特殊符号，至少6个字符，最多30个字符！");
        }
    }
}