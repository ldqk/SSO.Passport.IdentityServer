using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using IBLL;
using Masuit.Tools;
using Masuit.Tools.Net;
using Masuit.Tools.Strings;
using Models.Entity;
using SSO.Passport.IdentityServer.Models;
using SSO.Core;
using SSO.Core.Client;

namespace SSO.Passport.IdentityServer.Controllers
{
    [MyActionFilter, MyExceptionFilter]
    public class PassportController : Controller
    {
        private PassportService Passportservice => new PassportService();
        public IUserInfoBll UserInfoBll { get; set; }
        public PassportController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }

        public ActionResult PassportCenter()
        {
            //没有授权Token非法访问
            if (string.IsNullOrEmpty(Request["token"]))
            {
                return Content("没有授权Token，非法访问");
            }
            if (Session["user"] != null)
            {
                UserInfo userInfo = Session["user"] as UserInfo;
                return Redirect(Passportservice.GetReturnUrl(userInfo.Id.ToString(), Request["token"], Request["returnUrl"]));
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
            var success = Passportservice.AuthernVertify(Request["Token"], Convert.ToDateTime(Request["TimeStamp"]));
            if (!success)
            {
                return RedirectToAction("Login", new { ReturnUrl = Regex.Replace(Request["ReturnUrl"], @"ticket=(.{0,36})&token=(.{0,32})", String.Empty), Token = Request["Token"] });
            }
            return Redirect(Passportservice.GetReturnUrl(userId, Request["Token"], Request["ReturnUrl"]));
        }

        /// <summary>
        /// 统一登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            if (Session.GetSession<UserInfo>(Request.Cookies["sessionId"]?.Value) != null)
            {
                //已经登录系统了就直接跳到来源页
                return Redirect(Regex.Replace(Request["ReturnUrl"], @"ticket=(.{0,36})&token=(.{0,32})", String.Empty) ?? "/");
            }
            if (Request.Cookies.Count > 0)
            {
                string name = CookieHelper.GetCookieValue("username");
                string pwd = CookieHelper.GetCookieValue("password");
                UserInfo userInfo = UserInfoBll.Login(name, pwd);
                if (userInfo != null)
                {
                    Session["user"] = userInfo;
                    return RedirectToAction("PassportCenter", "Passport", new { Token = Request["Token"], ReturnUrl = Regex.Replace(Request["ReturnUrl"], @"ticket=(.{36})&token=(.{32})", String.Empty) });
                }
            }
            ViewBag.ReturnUrl = Regex.Replace(Request["ReturnUrl"], @"ticket=(.{0,36})&token=(.{0,32})", String.Empty);
            ViewBag.Token = Request["Token"];
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string valid, string remem)
        {
            string validSession = Session.GetSession<string>("valid") ?? String.Empty; //将验证码从Session中取出来，用于登录验证比较
            if (String.IsNullOrEmpty(validSession))
            {
                return Content("no:验证码错误");
            }
            Session.SetSession("valid", null); //验证成功就销毁验证码Session，非常重要
            if (!valid.Trim().Equals(validSession, StringComparison.InvariantCultureIgnoreCase))
            {
                return Content("no:验证码错误");
            }
            if (String.IsNullOrEmpty(username.Trim()) || String.IsNullOrEmpty(password.Trim()))
            {
                return Content("no:用户名或密码不能为空！");
            }
            var userInfo = UserInfoBll.Login(username, password);
            if (userInfo != null)
            {
                Session["user"] = userInfo;
                Response.Cookies.Add(new HttpCookie(Constants.USER_COOKIE_KEY) { HttpOnly = true, Value = userInfo.Id.ToString(), Expires = DateTime.Now.AddHours(2) });
                if (remem.Trim().Contains(new[] { "on", "true" })) //是否记住登录
                {
                    HttpCookie userCookie = new HttpCookie("username", Server.UrlEncode(username.Trim()));
                    Response.Cookies.Add(userCookie);
                    userCookie.Expires = DateTime.Now.AddDays(7);
                    HttpCookie passCookie = new HttpCookie("password", Server.UrlEncode(password.Trim())) { Expires = DateTime.Now.AddDays(7) };
                    Response.Cookies.Add(passCookie);
                }
                string sessionId = Guid.NewGuid().ToString();
                Session.SetSession(sessionId, userInfo);
                Response.Cookies.Add(new HttpCookie("sessionId", sessionId));
                if (Request["Token"].IsNullOrEmpty() || Request["ReturnUrl"].IsNullOrEmpty())
                {
                    return Content("ok::/");
                }
                var redirectUrl = Passportservice.GetReturnUrl(userInfo.Id.ToString(), Request["Token"], Request["ReturnUrl"]);
                return Content($"ok::{redirectUrl}");
            }
            return Content("no:用户名或密码错误");
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            Session["user"] = null;
            Response.Cookies[Constants.USER_COOKIE_KEY].Expires = DateTime.Now.AddDays(-1);
            return Content("退出成功");
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidateCode()
        {
            string code = Masuit.Tools.Strings.ValidateCode.CreateValidateCode(6);
            Session.SetSession("valid", code); //将验证码生成到Session中
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
            string validSession = Session["valid"] as string ?? String.Empty;
            if (String.IsNullOrEmpty(validSession))
            {
                return Content("no:验证码错误");
            }
            if (!code.Trim().Equals(validSession, StringComparison.InvariantCultureIgnoreCase))
            {
                return Content("no:验证码错误");
            }
            return Content("ok:验证码正确");
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="email"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="valid"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserRegister(string email, string username, string password, string valid)
        {
            string validSession = Session.GetSession<string>("valid") ?? String.Empty;
            if (String.IsNullOrEmpty(validSession))
            {
                return Content("no:验证码错误");
            }
            Session.SetSession("valid", null);
            if (!valid.Trim().Equals(validSession, StringComparison.InvariantCultureIgnoreCase))
            {
                return Content("no:验证码错误");
            }
            if (String.IsNullOrEmpty(email.Trim()))
            {
                return Content("no:邮箱不能为空");
            }
            if (String.IsNullOrEmpty(username.Trim()))
            {
                return Content("no:用户名不能为空");
            }
            if (String.IsNullOrEmpty(password.Trim()))
            {
                return Content("no:密码不能为空");
            }
            if (CheckExists(username.Trim()) || CheckExists(email.Trim()))
            {
                return Content("no:用户名或邮箱已存在");
            }
            UserInfo userInfo = new UserInfo() { Username = username.Trim(), Password = password.Trim() };
            userInfo = UserInfoBll.Register(userInfo);
            try
            {
                #region 新用户注册成功写入Cookie

                var userCookie = new HttpCookie("username", Server.UrlEncode(username.Trim()));
                Response.Cookies.Add(userCookie);
                userCookie.Expires = DateTime.Now.AddDays(7);
                var passCookie = new HttpCookie("password", Server.UrlEncode(password.Trim())) { Expires = DateTime.Now.AddDays(7) };
                Response.Cookies.Add(passCookie);
                string sessionId = Guid.NewGuid().ToString();
                Session.SetSession(sessionId, userInfo);
                Response.Cookies.Add(new HttpCookie("sessionId", sessionId));

                #endregion

                return Content($"ok:新用户{username.Trim()}注册成功！");
            }
            catch (Exception)
            {
                return Content($"no:{username.Trim()}注册失败！");
            }
        }

        /// <summary>
        /// 检查用户名存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserExists(string username) => CheckExists(username) ? Content($"no:用户名{username}已存在！") : Content($"ok:用户名{username}可以注册！");

        /// <summary>
        /// 检验用户名或邮箱是否存在
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool CheckExists(string str) => UserInfoBll.Any(u => u.Username.Equals(str.Trim(), StringComparison.InvariantCultureIgnoreCase) || u.Email.Equals(str.Trim(), StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// 检查邮箱存在
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public ActionResult EmailExists(string mail) => CheckExists(mail) ? Content("no:该邮箱已存在！") : Content("ok:该邮箱可以注册！");

    }
}