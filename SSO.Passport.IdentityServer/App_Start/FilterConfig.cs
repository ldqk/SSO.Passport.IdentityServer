using System.Web.Mvc;

namespace SSO.Passport.IdentityServer
{
    public class FilterConfig
    {
        /// <summary>
        /// 注册全局的过滤器
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new MyActionFilterAttribute());
            //filters.Add(new MyExceptionFilterAttribute());
        }
    }
}
