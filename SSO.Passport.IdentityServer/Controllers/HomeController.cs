using System.Web.Mvc;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            return Content("server running...");
        }
    }
}