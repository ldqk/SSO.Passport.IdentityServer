using System.Web.Mvc;

namespace SSO.Passport.IdentityServer.Models
{
    public class MyActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>在执行操作方法之前由 ASP.NET MVC 框架调用。</summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        /// <summary>在执行操作方法后由 ASP.NET MVC 框架调用。</summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        /// <summary>在执行操作结果之前由 ASP.NET MVC 框架调用。</summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        /// <summary>在执行操作结果后由 ASP.NET MVC 框架调用。</summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
    }
}