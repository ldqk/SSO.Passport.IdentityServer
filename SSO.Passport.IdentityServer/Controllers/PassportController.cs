using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Common;
using IBLL;
using Masuit.Tools;
using Masuit.Tools.Net;
using Masuit.Tools.Security;
using Masuit.Tools.Strings;
using Models.Dto;
using Models.Entity;
using Newtonsoft.Json;
using SSO.Core.Client;
using SSO.Core.Server;
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
        public ActionResult PassportCenter()
        {
            //没有授权Token非法访问
            if (string.IsNullOrEmpty(Request["token"]))
            {
                return Content("没有授权Token，非法访问");
            }
            if (Session.GetByCookieRedis<UserInfo>() != null)
            {
                UserInfo userInfo = Session.GetByCookieRedis<UserInfo>();
                return Redirect(PassportService.GetReturnUrl(userInfo.Id.ToString(), Request["token"], Request["returnUrl"]));
            }
            return View();
        }


        /// <summary>
        /// 授权登陆验证
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PassportVertify()
        {
            var cookie = Request.Cookies[Constants.USER_COOKIE_KEY];
            if (cookie == null || string.IsNullOrEmpty(cookie.ToString()))
            {
                return RedirectToAction("Login", new { ReturnUrl = Regex.Replace(Request["ReturnUrl"], @"ticket=(.{0,36})&token=(.{0,32})", String.Empty), Token = Request["Token"] });
            }
            string userId = cookie.Value;
            var success = PassportService.AuthernVertify(Request["Token"], Convert.ToDateTime(Request["TimeStamp"]));
            if (!success)
            {
                return RedirectToAction("Login", new { ReturnUrl = Regex.Replace(Request["ReturnUrl"], @"ticket=(.{0,36})&token=(.{0,32})", String.Empty), Token = Request["Token"] });
            }
            return Redirect(PassportService.GetReturnUrl(userId, Request["Token"], Request["ReturnUrl"]));
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

            if (Session.GetByCookieRedis<UserInfoDto>() != null)
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

#if !DEBUG
                HangfireHelper.CreateJob(typeof(IHangfireBackJob), "LoginRecord", "default", userInfo, Request.UserHostAddress); 
#endif
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
            UserInfoDto user = Session.GetByCookieRedis<UserInfoDto>();
#if DEBUG
            user = UserInfoBll.GetByUsername("masuit").Mapper<UserInfoDto>();
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
                UserInfoDto user = UserInfoBll.Register(new UserInfo() { Username = name, Password = pwd });
                if (user != null)
                {
                    return ResultData(user);
                }

                return ResultData(null, false, "用户注册失败！");
            }

            return ResultData(null, false, "密码强度值不够，密码必须包含数字，必须包含小写或大写字母，必须包含至少一个特殊符号，至少6个字符，最多30个字符！");
        }

        /// <summary>
        /// 获取用户信息、访问控制权限、菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUser()
        {
            UserInfoDto user = Session.GetByCookieRedis<UserInfoDto>();
            string appid = ConfigurationManager.AppSettings["AppId"];
            List<ControlOutputDto> acl = UserInfoBll.GetAccessControls(appid, user.Id);
            List<MenuOutputDto> menus = UserInfoBll.GetMenus(appid, user.Id);
            return Json(new { user, acl, menus });
        }

        /// <summary>
        /// 获取登陆记录
        /// </summary>
        /// <param name="token"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ActionResult LoginRecord(string token, int page = 1, int size = 10)
        {
            UserInfoDto user = Session.GetByCookieRedis<UserInfoDto>();
            List<LoginRecordDto> list = LoginRecordBll.LoadPageEntitiesNoTracking<DateTime, LoginRecordDto>(page, size, out int total, r => r.UserInfoId.Equals(user.Id), r => r.LoginTime, false).ToList();
            int pages = (int)Math.Ceiling(total * 1.0 / size);
            return ResultData(new { list, pages });
        }
    }
}