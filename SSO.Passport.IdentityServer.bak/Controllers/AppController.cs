using System.Web.Mvc;
using IBLL;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class AppController : BaseController
    {
        public IClientAppBll ClientAppBll { get; set; }
        public AppController(IClientAppBll clientAppBll)
        {
            ClientAppBll = clientAppBll;
        }

        public ActionResult NAME()
        {

            return View();
        }
    }
}