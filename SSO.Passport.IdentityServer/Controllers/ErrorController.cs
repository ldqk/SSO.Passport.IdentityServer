using System.Web.Mvc;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error")]
        public ActionResult Index()
        {
            return Content("503 Service Unavailable！");
        }
    }
}