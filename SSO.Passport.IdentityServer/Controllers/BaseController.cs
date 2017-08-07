using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IBLL;
using Models.Entity;
using Newtonsoft.Json;
using SSO.Passport.IdentityServer.Models;

namespace SSO.Passport.IdentityServer.Controllers
{
#if DEBUG
    using Core.Client;

    [Authority(Code = AuthCodeEnum.Login), MyExceptionFilter]
#else
    [MyExceptionFilter]
#endif
    public class BaseController : Controller
    {
        public IUserInfoBll UserInfoBll { get; set; }

        public ActionResult ResultData(object data, bool isTrue = true, string message = "")
        {
            return Content(JsonConvert.SerializeObject(new { Success = isTrue, IsLogin = true, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }));
        }

        /// <summary>在调用操作方法前调用。</summary>
        /// <param name="filterContext">有关当前请求和操作的信息。</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;
            var method = request.HttpMethod;
#if DEBUG
            UserInfo userInfo = UserInfoBll.GetByUsername("admin");
#else
            UserInfo userInfo = Session.GetByCookieRedis<UserInfo>();
#endif
            if (userInfo != null)
            {
                IList<Function> FunctionList = UserInfoBll.GetPermissionList(userInfo);
                if (!FunctionList.Any(c => c.Controller.Equals(controller, StringComparison.InvariantCultureIgnoreCase) && c.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase) && c.HttpMethod.Equals(method, StringComparison.InvariantCultureIgnoreCase)))
                {
                    filterContext.Result = new JsonResult() { Data = new { Success = false, Message = "无权限访问！" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
                }
            }
        }
    }
}