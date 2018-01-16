using System.Web.Mvc;

namespace SSO.Passport.IdentityServer.Models
{
    public class MyExceptionFilterAttribute : HandleErrorAttribute
    {
        /// <summary>在发生异常时调用。</summary>
        /// <param name="filterContext">操作筛选器上下文。</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="filterContext" /> 参数为 null。</exception>
        public override void OnException(ExceptionContext filterContext)
        {
#if !DEBUG
            LogManager.Error(filterContext.Exception.Source, filterContext.Exception);
            filterContext.HttpContext.Response.StatusCode = 503;
            if (filterContext.HttpContext.Request.HttpMethod.ToLower().Equals("get"))
            {
                filterContext.Result = new RedirectResult("/ServiceUnavailable"); //new ErrorController().ServiceUnavailable();
            }
            else
            {
                filterContext.Result = new JsonResult() { ContentEncoding = Encoding.UTF8, ContentType = "application/json", Data = new { StatusCode = 500, Success = false, Message = "服务器发生错误！" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            filterContext.ExceptionHandled = true; //设置异常已经处理  
#endif
        }
    }
}