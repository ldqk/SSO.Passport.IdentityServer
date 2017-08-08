using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using IBLL;
using Masuit.Tools;
using Masuit.Tools.Net;
using Masuit.Tools.NoSQL;
using Masuit.Tools.Security;
using Masuit.Tools.Strings;
using Models.Dto;
using Models.Entity;
using Newtonsoft.Json;
using SSO.Core.Client;
using SSO.Core.Server;
using SSO.Passport.IdentityServer.Models;

namespace SSO.Passport.IdentityServer.Controllers
{
    [MyExceptionFilter]
    public class PassportController : Controller
    {
        private PassportService Passportservice => new PassportService();
        public IUserInfoBll UserInfoBll { get; set; }
        public RedisHelper RedisHelper { get; set; }
        public PassportController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
            RedisHelper = new RedisHelper();
        }

        public ActionResult ResultData(object data, bool isTrue = true, string message = "")
        {
            return Content(JsonConvert.SerializeObject(new { Success = isTrue, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }));
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
            if (Session.GetByCookieRedis<UserInfo>() != null)
            {
                //已经登录系统了就直接跳到来源页
                return Redirect(Regex.Replace(Request["ReturnUrl"], @"ticket=(.{0,36})&token=(.{0,32})", String.Empty) ?? "/");
            }
            if (Request.Cookies.Count > 0)
            {
                string name = CookieHelper.GetCookieValue("username");
                string pwd = CookieHelper.GetCookieValue("password").AESDecrypt();
                UserInfo userInfo = UserInfoBll.Login(name, pwd);
                if (userInfo != null)
                {
                    Session.SetByRedis(userInfo.MapTo<UserInfoLoginModel>());
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
            string validSession = Session.GetByCookieRedis<string>("valid", 1) ?? String.Empty; //将验证码从Session中取出来，用于登录验证比较
            if (String.IsNullOrEmpty(validSession))
            {
                return Content("no:验证码错误");
            }
            Session.RemoveByCookieRedis("valid"); //验证成功就销毁验证码Session，非常重要
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
                Session.SetByRedis(userInfo.MapTo<UserInfoLoginModel>());
                Response.Cookies.Add(new HttpCookie(Constants.USER_COOKIE_KEY) { HttpOnly = true, Value = userInfo.Id.ToString(), Expires = DateTime.Now.AddHours(2) });
                if (remem.Trim().Contains(new[] { "on", "true" })) //是否记住登录
                {
                    HttpCookie userCookie = new HttpCookie("username", Server.UrlEncode(username.Trim()));
                    Response.Cookies.Add(userCookie);
                    userCookie.Expires = DateTime.Now.AddDays(7);
                    HttpCookie passCookie = new HttpCookie("password", Server.UrlEncode(password.Trim().AESEncrypt())) { Expires = DateTime.Now.AddDays(7) };
                    Response.Cookies.Add(passCookie);
                }
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
        public ActionResult LogOut(string returnUrl)
        {
            Session.RemoveByCookieRedis();
            Response.Cookies[Constants.USER_COOKIE_KEY].Expires = DateTime.Now.AddDays(-1);
            return Redirect(returnUrl);
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidateCode()
        {
            string code = Masuit.Tools.Strings.ValidateCode.CreateValidateCode(6);
            Session.SetByRedis(code, "valid", 1); //将验证码生成到Session中
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
            string validSession = Session.GetByRedis<string>("valid", 1);
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
        public ActionResult Register(UserInfoInputDto model, string valid)
        {
            string validSession = Session.GetByCookieRedis<string>("valid", 1) ?? String.Empty;
            if (String.IsNullOrEmpty(validSession))
            {
                return Content("no:验证码错误");
            }
            Session.RemoveByCookieRedis("valid");
            if (!valid.Trim().Equals(validSession, StringComparison.InvariantCultureIgnoreCase))
            {
                return Content("no:验证码错误");
            }
            if (String.IsNullOrEmpty(model.Email.Trim()))
            {
                return ResultData(model, false, $"邮箱不能为空！");
            }
            if (String.IsNullOrEmpty(model.Username.Trim()))
            {
                return ResultData(model, false, $"用户名不能为空！");
            }
            if (String.IsNullOrEmpty(model.Password.Trim()))
            {
                return ResultData(model, false, $"密码不能为空！");
            }
            if (UserInfoBll.UsernameExist(model.Username))
            {
                return ResultData(model, false, $"用户名{model.Username}已经存在！");
            }
            if (UserInfoBll.EmailExist(model.Email))
            {
                return ResultData(model, false, $"邮箱{model.Email}已经存在！");
            }
            if (UserInfoBll.PhoneExist(model.PhoneNumber))
            {
                return ResultData(model, false, $"电话号码{model.PhoneNumber}已经存在！");
            }
            UserInfo userInfo = UserInfoBll.Register(Mapper.Map<UserInfo>(model));
            try
            {
                #region 新用户注册成功写入Cookie

                var userCookie = new HttpCookie("username", Server.UrlEncode(model.Username.Trim()));
                Response.Cookies.Add(userCookie);
                userCookie.Expires = DateTime.Now.AddDays(7);
                var passCookie = new HttpCookie("password", Server.UrlEncode(model.Password.Trim()).AESEncrypt()) { Expires = DateTime.Now.AddDays(7) };
                Response.Cookies.Add(passCookie);
                Session.SetByRedis(userInfo.MapTo<UserInfoLoginModel>());

                #endregion

                return ResultData($"新用户{model.Username.Trim()}注册成功！", message: $"新用户{model.Username.Trim()}注册成功！");
            }
            catch (Exception)
            {
                return ResultData($"{model.Username.Trim()}注册失败！", message: $"{model.Username.Trim()}注册失败！");
            }
        }

        /// <summary>
        /// 检查用户名存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserExists(string username) => UserInfoBll.UsernameExist(username) ? ResultData($"用户名{username}已存在！", message: $"用户名{username}已存在！") : ResultData($"用户名{username}可以注册！", message: $"用户名{username}可以注册！");

        /// <summary>
        /// 检查邮箱存在
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmailExists(string mail) => UserInfoBll.EmailExist(mail) ? ResultData($"邮箱{mail}已存在！", message: $"邮箱{mail}已存在！") : ResultData($"该邮箱可以注册！", message: "$该邮箱可以注册！");

        /// <summary>
        /// 检查手机号码
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PhoneExists(string num) => UserInfoBll.PhoneExist(num) ? ResultData($"手机号码{num}已存在！", message: $"手机号码{num}已存在！") : ResultData($"该手机号码可以注册！", message: "$该手机号码可以注册！");

    }
}