using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SSO.Core;
using SSO.Core.Client;

namespace WebAppClient.Controllers
{
    [Authority(Code = AuthCodeEnum.Login)]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            AuthernUtil.Logout();
            return Content(LoginService.Logout("http://www.baidu.com"));
        }

        public ActionResult ChangePassword(string id, string old, string pwd, string pwd2)
        {
            string data = AuthernUtil.CallServerApi("/Api/ChangePassword", new Dictionary<string, string>()
            {
                { nameof(id), id },
                {nameof(old),old },
                {nameof(pwd),pwd },
                {nameof(pwd2),pwd2 }
            });
            return Content(data);
        }
    }
}