using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IBLL;
using Masuit.Tools;
using Models.Entity;
using Newtonsoft.Json;
using SSO.Core.Client;
using SSO.Passport.IdentityServer.Models;
#if !DEBUG
using Masuit.Tools.Net; 
#endif

namespace SSO.Passport.IdentityServer.Controllers
{
    [Authority(Code = AuthCodeEnum.Login)]
    [MyExceptionFilter]
    public class BaseController : Controller
    {
        public IUserInfoBll UserInfoBll { get; set; }

        public ActionResult ResultData(object data, bool isTrue = true, string message = "")
        {
            return Content(JsonConvert.SerializeObject(new { Success = isTrue, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }));
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
                if (!userInfo.Username.ToLower().Contains(new[] { "admin", "sa", "system", "root", "everyone" }) && !FunctionList.Any(c => c.Controller.Equals(controller, StringComparison.InvariantCultureIgnoreCase) && c.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase) && c.HttpMethod.Equals(method, StringComparison.InvariantCultureIgnoreCase)))
                {//如果不是系统账户，并且不包含以上权限，则阻断
                    filterContext.Result = new JsonResult { Data = new { Success = false, Message = "无权限访问！" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
                }
                ViewBag.FunctionList = FunctionList;
            }
        }
    }
}