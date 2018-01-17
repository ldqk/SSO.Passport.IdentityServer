using System.Web.Mvc;
using SSO.Passport.IdentityServer.Models;

namespace SSO.Passport.IdentityServer.Controllers
{
    [MyActionFilter, MyExceptionFilter]
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [Route("filemanager")]
        public ActionResult FileManager()
        {

            return View();
        }
    }
}