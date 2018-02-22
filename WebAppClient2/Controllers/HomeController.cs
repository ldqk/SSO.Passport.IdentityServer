using System.Web.Mvc;
using SSO.Core.Client;
using SSO.Core.Filter;
using SSO.Core.Model;

namespace WebAppClient2.Controllers
{
    [Authority(Code = AuthCodeEnum.Login)]
    [PermissionFilter(Granularity = PermissionGranularity.Action)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Acl()
        {
            UserModel userModel = LoginService.GetAccessControls(AuthernUtil.CurrentUser.Id);
            return Json(userModel, JsonRequestBehavior.AllowGet);
        }
    }
}