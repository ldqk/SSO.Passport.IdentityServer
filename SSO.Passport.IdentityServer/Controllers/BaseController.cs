using System;
using System.Configuration;
using System.Web.Mvc;
using Autofac;
using Common;
using IBLL;
using Masuit.Tools.Net;
using Masuit.Tools.NoSQL;
using Masuit.Tools.Security;
using Models.Dto;
using SSO.Passport.IdentityServer.Models;

namespace SSO.Passport.IdentityServer.Controllers
{
    [Authority]
    public class BaseController : Controller
    {
        public IUserInfoBll UserInfoBll { get; set; } = AutofacConfig.Container.Resolve<IUserInfoBll>();
        public static RedisHelper RedisHelper { get; set; } = new RedisHelper();
        public UserInfoOutputDto CurrentUser { get; set; }

        /// <summary>在调用操作方法前调用。</summary>
        /// <param name="filterContext">有关当前请求和操作的信息。</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UserInfoOutputDto user = filterContext.HttpContext.Session.GetByCookieRedis<UserInfoOutputDto>();

            if (user == null && Request.Cookies.Count > 2)
            {
                //走网页登陆通道
                string name = CookieHelper.GetCookieValue("username");
                string pwd = CookieHelper.GetCookieValue("password")?.DesDecrypt(ConfigurationManager.AppSettings["BaiduAK"]);
                var userInfo = UserInfoBll.Login(name, pwd);
                if (userInfo != null)
                {
                    CookieHelper.SetCookie("username", name, DateTime.Now.AddDays(7));
                    CookieHelper.SetCookie("password", CookieHelper.GetCookieValue("password"), DateTime.Now.AddDays(7));
                    Session.SetByRedis(userInfo);
                    CurrentUser = userInfo;
                }
            }
#if DEBUG
            user = AutofacConfig.Container.Resolve<IUserInfoBll>().GetByUsername("admin").Mapper<UserInfoOutputDto>();
            Session.SetByRedis(user);
            CurrentUser = user;
#endif
        }
    }
}