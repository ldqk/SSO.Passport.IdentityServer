using System;
using System.Text;
using System.Web.Mvc;
using Masuit.Tools.Logging;

namespace SSO.Passport.IdentityServer.Models
{
    /// <summary>
    /// 自定义异常过滤器
    /// </summary>
    public class MyExceptionFilterAttribute : HandleErrorAttribute
    {
        /// <summary>在发生异常时调用。</summary>
        /// <param name="filterContext">操作筛选器上下文。</param>
        /// <exception cref="ArgumentException">该对象必须为运行时 <see cref="N:System.Reflection" /> 对象</exception>
        public override void OnException(ExceptionContext filterContext)
        {
            LogManager.Error(filterContext.Exception.Source, filterContext.Exception);
            filterContext.Result = new JsonResult
            {
                Data = new { Success = false, Message = "服务器发生错误！" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json"
            };
        }
    }
}