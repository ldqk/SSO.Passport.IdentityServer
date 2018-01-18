using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using IBLL;
using Models.Dto;
using Models.Entity;

namespace SSO.Passport.IdentityServer.Controllers
{
    /// <summary>
    /// 公共api
    /// </summary>
    public class PublicController : Controller
    {
        public IUserInfoBll UserInfoBll { get; set; }
        public PublicController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }

        public ActionResult GetAccessControls()
        {
            UserInfo userInfo = UserInfoBll.GetByUsername("admin");
            List<MenuOutputDto> controls = UserInfoBll.GetMenus(ConfigurationManager.AppSettings["AppId"], userInfo.Id);
            return Json(controls, JsonRequestBehavior.AllowGet);
        }
    }
}