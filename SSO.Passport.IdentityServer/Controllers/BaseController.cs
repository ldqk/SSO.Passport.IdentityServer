using System;
using System.Text;
using System.Web.Mvc;
using Autofac;
using IBLL;
using Masuit.Tools.NoSQL;
using Models.Dto;
using Models.ViewModel;
using Newtonsoft.Json;
using SSO.Core.Client;

namespace SSO.Passport.IdentityServer.Controllers
{
    [Authority(Code = AuthCodeEnum.Login), PermissionFilter(Granularity = PermissionGranularity.Action)]
    public class BaseController : Controller
    {
        protected IUserInfoBll UserInfoBll { get; set; } = AutofacConfig.Container.Resolve<IUserInfoBll>();
        protected IClientAppBll ClientAppBll { get; set; } = AutofacConfig.Container.Resolve<IClientAppBll>();
        protected IRoleBll RoleBll { get; set; } = AutofacConfig.Container.Resolve<IRoleBll>();
        protected IPermissionBll PermissionBll { get; set; } = AutofacConfig.Container.Resolve<IPermissionBll>();
        protected IMenuBll MenuBll { get; set; } = AutofacConfig.Container.Resolve<IMenuBll>();
        protected IUserGroupBll UserGroupBll { get; set; } = AutofacConfig.Container.Resolve<IUserGroupBll>();
        protected IControlBll ControlBll { get; set; } = AutofacConfig.Container.Resolve<IControlBll>();
        protected static RedisHelper RedisHelper { get; set; } = new RedisHelper();
        public UserInfoDto CurrentUser { get; set; }

        protected ActionResult ResultData(object data, bool isTrue = true, string message = "", bool isLogin = true)
        {
            return Content(JsonConvert.SerializeObject(new { IsLogin = isLogin, Success = isTrue, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }), "application/json", Encoding.UTF8);
        }

        protected ActionResult PageResult(object data, int size, int total)
        {
            int pageCount = (int)Math.Ceiling(total * 1.0 / size);
            return Content(JsonConvert.SerializeObject(new PageDataModel(data, pageCount, total), new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }), "application/json", Encoding.UTF8);
        }

        /// <summary>在调用操作方法前调用。</summary>
        /// <param name="filterContext">有关当前请求和操作的信息。</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
#if !DEBUG
            var user = UserInfoBll.GetByUsername("admin").Mapper<UserInfoDto>();
            CurrentUser = user;
            Session.SetByRedis(user);
#endif
        }
    }
}