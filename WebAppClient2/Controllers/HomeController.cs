using System.Web.Mvc;
using SSO.Core.Client;
using SSO.Core.Model;

namespace WebAppClient2.Controllers
{
    public class HomeController : Controller
    {
        [Authority(Code = AuthCodeEnum.Login)]
        public ActionResult Index()
        {
            return View();
        }
        [PermissionFilter(Granularity = PermissionGranularity.Action)]
        public ActionResult Acl()
        {
            UserModel userModel = LoginService.GetAccessControls(AuthernUtil.CurrentUser.Id);
            return Json(userModel, JsonRequestBehavior.AllowGet);
        }
    }
}