using System.Web.Mvc;
using SSO.Core.Client;

namespace WebAppClient2.Controllers
{
    public class HomeController : Controller
    {
        [Auth(Code = AuthCodeEnum.Login)]
        public ActionResult Index()
        {
            return View();
        }
    }
}