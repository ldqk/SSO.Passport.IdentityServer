using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Masuit.Tools;
using SSO.Passport.IdentityServer.Models;

namespace SSO.Passport.IdentityServer.Controllers
{
    [MyActionFilter, MyExceptionFilter]
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(Request.Url?.Query))
            {
                return Redirect("/");
            }
            return View();
        }

        [Route("filemanager")]
        public ActionResult FileManager()
        {

            return View();
        }
        [Route("apis")]
        public ActionResult Apis()
        {
            IList<Tuple<string, string>> list = new List<Tuple<string, string>>();
            Assembly.GetExecutingAssembly().GetTypes().Where(t => t.FullName.EndsWith("Controller")).ForEach(t => t.GetMethods().Where(m => m.IsPublic && m.ReturnType.IsAssignableFrom(typeof(ActionResult)) && !m.Name.StartsWith("get_")).ForEach(m => list.Add(new Tuple<string, string>(t.Name.Substring(0, t.Name.IndexOf("Controller", StringComparison.Ordinal)), m.Name))));
            return View(list);
        }
    }
}