using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using IBLL;
using Masuit.Tools;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }

        public ActionResult Index()
        {
            IList<Tuple<string, string>> list = new List<Tuple<string, string>>();
            Assembly.GetExecutingAssembly().GetTypes().Where(t => t.FullName.EndsWith("Controller")).ForEach(t => t.GetMethods().Where(m => m.IsPublic && m.ReturnType.IsAssignableFrom(typeof(ActionResult)) && !m.Name.StartsWith("get_")).ForEach(m => list.Add(new Tuple<string, string>(t.Name.Substring(0, t.Name.IndexOf("Controller", StringComparison.Ordinal)), m.Name))));
            return View(list);
        }
    }
}