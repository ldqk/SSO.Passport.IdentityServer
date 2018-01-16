using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Autofac;
using IBLL;
using Masuit.Tools.Net;
using Masuit.Tools.NoSQL;
using Masuit.Tools.Security;
using Models.Dto;

namespace SSO.Passport.IdentityServer.Models
{
    public class AuthorityAttribute : ActionFilterAttribute
    {
        public static RedisHelper RedisHelper { get; set; } = new RedisHelper();
        /// <summary>在执行操作方法之前由 ASP.NET MVC 框架调用。</summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0)
            {
                filterContext.HttpContext.SkipAuthorization = true;
                return;
            }
#if DEBUG
            UserInfoOutputDto user = filterContext.HttpContext.Session.GetByCookieRedis<UserInfoOutputDto>();
            if (user == null)
            {
                //走API登录通道
                string token = filterContext.HttpContext.Request["token"] ?? filterContext.Controller.ValueProvider.GetValue("token")?.AttemptedValue ?? filterContext.HttpContext.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(token) && RedisHelper.KeyExists(token))
                {
                    user = RedisHelper.GetString<UserInfoOutputDto>(token);
                    RedisHelper.Expire(token, TimeSpan.FromMinutes(20));
                    filterContext.HttpContext.Session.SetByRedis(user);
                }
            }
            if (user == null)
            {
                //先尝试自动登录
                if (filterContext.HttpContext.Request.Cookies.Count > 2)
                {
                    string name = CookieHelper.GetCookieValue("username");
                    string pwd = CookieHelper.GetCookieValue("password")?.DesDecrypt(ConfigurationManager.AppSettings["BaiduAK"]);
                    var userInfo = AutofacConfig.Container.Resolve<IUserInfoBll>().Login(name, pwd);
                    if (userInfo != null)
                    {
                        CookieHelper.SetCookie("username", name, DateTime.Now.AddDays(7));
                        CookieHelper.SetCookie("password", CookieHelper.GetCookieValue("password"), DateTime.Now.AddDays(7));
                        filterContext.HttpContext.Session.SetByRedis(userInfo);
                    }
                    else
                    {
                        if (filterContext.HttpContext.Request.HttpMethod.ToLower().Equals("get"))
                        {
                            filterContext.Result = new RedirectResult("/passport/login?from=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url?.ToString())?.Replace("#", "%23"));
                        }
                        else
                        {
                            filterContext.Result = new JsonResult { ContentEncoding = Encoding.UTF8, ContentType = "application/json", Data = new { StatusCode = 200, Success = false, IsLogin = false, Message = "未登录系统，请先登录！" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                        }
                    }
                }
                else
                {
                    if (filterContext.HttpContext.Request.HttpMethod.ToLower().Equals("get"))
                    {
                        filterContext.Result = new RedirectResult("/passport/login?from=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url?.ToString()));
                    }
                    else
                    {
                        filterContext.Result = new JsonResult { ContentEncoding = Encoding.UTF8, ContentType = "application/json", Data = new { StatusCode = 200, Success = false, IsLogin = false, Message = "未登录系统，请先登录！" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
            }
#endif
        }
    }
}