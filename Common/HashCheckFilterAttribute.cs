using System;
using System.Configuration;
using System.Text;
using System.Web.Mvc;
using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Security;

namespace Common
{
    /// <summary>
    /// URL的时效性、参数hash合法性校验过滤，保证URL在30秒内才有效
    /// </summary>
    public class HashCheckFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sec = DateTime.Now.GetTotalSeconds(); //获取当前的时间戳
            var isGet = filterContext.RequestContext.HttpContext.Request.HttpMethod.ToLower().Equals("get"); //判断请求方式
            var time = isGet ? filterContext.HttpContext.Request["time"] ?? String.Empty : filterContext.Controller.ValueProvider.GetValue("time").AttemptedValue; //获取请求参数带过来的时间戳
            var hash = isGet ? filterContext.HttpContext.Request["hash"] ?? String.Empty : filterContext.Controller.ValueProvider.GetValue("hash").AttemptedValue; //获取请求参数的hash值
            if (string.IsNullOrEmpty(time) || string.IsNullOrEmpty(hash)) //先判空，若空则截断本次请求
            {
                filterContext.Result = new JsonResult() { Data = new { Success = false, Message = "URL参数不完整！" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
            }
            else if (sec - time.ToInt32() > 30) //然后时效性检查，URL在30秒内有效，若超时，则截断本次请求
            {
                filterContext.Result = new JsonResult()
                {
                    Data = new
                    {
                        Success = false,
                        Message = "该URL已经失效！"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json"
                };
            }
            else //最后URL有效的执行逻辑
            {
                string salt = ConfigurationManager.AppSettings["encryptSalt"] ?? "masuit".DesEncrypt(); //获取加密盐
                string hash2 = (time + salt).MDString(); //将请求参数的时间戳与加密盐一起进行hash
                if (!hash.Equals(hash2, StringComparison.InvariantCultureIgnoreCase)) //对比服务器计算的hash与请求参数带过来的hash是否一致，忽略大小写
                {
                    //如果不一致，也截断本次请求
                    filterContext.Result = new JsonResult() { Data = new { Success = false, Message = "URL无效！" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
                }
            }
            //如果hash一致，则放行
        }
    }
}