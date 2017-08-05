using System;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace SSO.Core.Client
{
    public enum AuthCodeEnum
    {
        Public = 1,
        Login = 2
    }

    /// <summary>
    /// 授权过滤器
    /// </summary>
    public class AuthAttribute : ActionFilterAttribute
    {
        /// <summary> 
        /// 权限代码 
        /// </summary> 
        public AuthCodeEnum Code { get; set; }

        /// <summary> 
        /// 验证权限
        /// </summary> 
        /// <param name="filterContext"></param> 
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var session = filterContext.HttpContext.Session;
            //如果不存在身份信息 
            if (AuthernUtil.CurrentUser == null)
            {
                if (Code == AuthCodeEnum.Public)
                {
                    return;
                }
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
                    //Token加入缓存中，设计过期时间为20分钟
                    cache.Add(Constants.TOKEN_KEY, tokenModel, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    filterContext.Result = new ContentResult { Content = GetAuthernScript(AuthernUtil.GetAuthorityUrl(tokenModel.Token, timestamp), returnUrl) };
                    return;
                }
                session[Constants.USER_SESSION_KEY] = LoginService.GetUserInfo(ticket);
                //验证通过,cache中去掉Token,保证每个token只能使用一次
                cache.Remove(Constants.TOKEN_KEY);
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