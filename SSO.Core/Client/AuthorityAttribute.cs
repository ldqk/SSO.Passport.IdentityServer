using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Net;
using Masuit.Tools.Security;

namespace SSO.Core.Client
{
    public enum AuthCodeEnum
    {
        /// <summary>
        /// 开放
        /// </summary>
        Public,
        /// <summary>
        /// 登录校验
        /// </summary>
        Login,
        /// <summary>
        /// hash校验
        /// </summary>
        HashCheck
    }

    /// <summary>
    /// 授权过滤器，提供了三种方式的授权，默认为不授权，若需要授权则配置Code属性
    /// </summary>
    public class AuthorityAttribute : ActionFilterAttribute
    {
        /// <summary> 
        /// 权限代码，默认为<see cref="AuthCodeEnum.Public"/>，若为<see cref="AuthCodeEnum.HashCheck"/>，则需要传time和hash参数，time为当前时间到1970-01-01 00:00:00的秒数，hash为time和加密盐拼接后的MD5一次加密结果，加密盐需要在config文件中AppSettings节点下键为encryptSalt，若未配置，则取“masuit”的DES默认加密结果；<br/>
        /// 若为<see cref="AuthCodeEnum.Login"/>，则使用用户名登录校验，登录过程会自动生成token和ticket，ticket即为用户的id标识。
        /// </summary> 
        public AuthCodeEnum Code { get; set; }

        /// <summary> 
        /// 验证权限
        /// </summary> 
        /// <param name="filterContext"></param> 
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Code is AuthCodeEnum.Login)
            {
                var request = filterContext.HttpContext.Request;
                var session = filterContext.HttpContext.Session;
                //如果不存在身份信息 
                if (AuthernUtil.CurrentUser == null)
                {
                    string reqToken = request["token"];
                    string ticket = request["ticket"];
                    Cache cache = HttpContext.Current.Cache;
                    //每次刷新页面的时候首先删除Token
                    if (string.IsNullOrEmpty(reqToken) || string.IsNullOrEmpty(ticket))
                    {
                        cache.Remove(Constants.TOKEN_KEY);
                    }
                    //没有获取到Token或者Token验证不通过或者没有取到从P回调的ticket 都进行再次请求P
                    TokenModel tokenModel = cache.Get(Constants.TOKEN_KEY) == null ? null : (TokenModel)cache.Get(Constants.TOKEN_KEY);
                    if (string.IsNullOrEmpty(reqToken) || tokenModel == null || tokenModel.Token != reqToken || string.IsNullOrEmpty(ticket))
                    {
                        DateTime timestamp = DateTime.Now;
                        string returnUrl = request.Url.AbsoluteUri;
                        tokenModel = new TokenModel { TimeStamp = timestamp, Token = AuthernUtil.CreateToken(timestamp) };
                        //Token加入缓存中，设计过期时间为20分钟，这里为了方便设置Token的过期时间，所以使用Cache来存取Token,设定Token的失效时间为20分钟，当验证成功则从cache中移除Token。
                        cache.Add(Constants.TOKEN_KEY, tokenModel, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                        filterContext.Result = new ContentResult { Content = GetAuthernScript(AuthernUtil.GetAuthorityUrl(tokenModel.Token, timestamp), returnUrl) };
                        return;
                    }
                    session.SetByRedis(LoginService.GetUserInfo(ticket), Constants.USER_SESSION_KEY);
                    //验证通过,cache中去掉Token,保证每个token只能使用一次
                    cache.Remove(Constants.TOKEN_KEY);
                }
            }
            else if (Code is AuthCodeEnum.HashCheck)
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
                    filterContext.Result = new JsonResult() { Data = new { Success = false, Message = "该URL已经失效！" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, ContentEncoding = Encoding.UTF8, ContentType = "application/json" };
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

        private string GetAuthernScript(string authernUrl, string returnUrl)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.Append("<script type='text/javascript'>");
            sbScript.AppendFormat($"window.location.href='{authernUrl}&returnUrl=' + encodeURIComponent('{returnUrl}');");
            sbScript.Append("</script>");
            return sbScript.ToString();
        }
    }
}