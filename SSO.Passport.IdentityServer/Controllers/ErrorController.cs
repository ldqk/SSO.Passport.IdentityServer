using System.Web.Mvc;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return Content("404 error");
        }
    }
}