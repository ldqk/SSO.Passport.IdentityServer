using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Masuit.Tools;
using SSO.Core.Client;
using SSO.Core.Model;

namespace SSO.Core.Filter
{
    public enum PermissionGranularity
    {
        /// <summary>
        /// 请求方式级别
        /// </summary>
        RequestMethod,

        /// <summary>
        /// 方法级
        /// </summary>
        Action,

        /// <summary>
        /// 控制器级
        /// </summary>
        Controller,

        /// <summary>
        /// 请求路径级
        /// </summary>
        UrlPath
    }

    public class PermissionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 权限粒度
        /// </summary>
        public PermissionGranularity Granularity { get; set; } = PermissionGranularity.Action;

        /// <summary>在执行操作方法之前由 ASP.NET MVC 框架调用。</summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0)
            {
                filterContext.HttpContext.SkipAuthorization = true;
                return;
            }
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;
            string method = filterContext.HttpContext.Request.HttpMethod;
            string path = filterContext.HttpContext.Request.Path;

            UserInfoLoginModel userInfo = AuthernUtil.CurrentUser;
            if (userInfo != null)
            {
                if (userInfo.Username.ToLower().Contains(new[] { "admin", "sa", "system", "root", "everyone" }))
                {
                    return;
                }
                string token = filterContext.HttpContext.Request.Headers["Authorization"] ?? filterContext.HttpContext.Request["token"] ?? userInfo.Id;
                UserModel userModel = LoginService.GetAccessControls(token);

                switch (Granularity)
                {
                    case PermissionGranularity.RequestMethod:
                        if (!userModel.Acl.Any(c => c.IsAvailable && c.HttpMethod.Equals(method, StringComparison.InvariantCultureIgnoreCase) && ((c.Controller != null && c.Controller.Equals(controller, StringComparison.InvariantCultureIgnoreCase) && c.Action != null && c.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase)))))
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new
                                {
                                    Success = false,
                                    Message = "无权限访问！"
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                ContentEncoding = Encoding.UTF8,
                                ContentType = "application/json"
                            };
                        }
                        return;
                    case PermissionGranularity.Action:
                        if (!userModel.Acl.Any(c => c.IsAvailable && ((c.Controller != null && c.Controller.Equals(controller, StringComparison.InvariantCultureIgnoreCase) && c.Action != null && c.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase)))))
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new
                                {
                                    Success = false,
                                    Message = "无权限访问！"
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                ContentEncoding = Encoding.UTF8,
                                ContentType = "application/json"
                            };
                        }
                        return;
                    case PermissionGranularity.Controller:
                        if (!userModel.Acl.Any(c => c.IsAvailable && (c.Controller != null && c.Controller.Equals(controller, StringComparison.InvariantCultureIgnoreCase))))
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new
                                {
                                    Success = false,
                                    Message = "无权限访问！"
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                ContentEncoding = Encoding.UTF8,
                                ContentType = "application/json"
                            };
                        }
                        return;
                    case PermissionGranularity.UrlPath:
                        if (!userModel.Acl.Any(c => c.IsAvailable && path.Contains(c.Path)))
                        {
                            filterContext.Result = new JsonResult
                            {
                                Data = new
                                {
                                    Success = false,
                                    Message = "无权限访问！"
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                ContentEncoding = Encoding.UTF8,
                                ContentType = "application/json"
                            };
                        }
                        return;
                    default:
                        return;
                }
            }
            else
            {
                filterContext.Result = new JsonResult()
                {
                    Data = new ResponseModel()
                    {
                        Message = "登录态丢失或未登录系统，请先登录！",
                        Success = false
                    },
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}