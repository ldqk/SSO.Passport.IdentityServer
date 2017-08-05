using System.Web.Mvc;
using Newtonsoft.Json;
using SSO.Passport.IdentityServer.Models;

namespace SSO.Passport.IdentityServer.Controllers
{
#if !DEBUG
    using SSO.Core.Client;
    
    [Authority(Code = AuthCodeEnum.Login), MyExceptionFilter]
#else
    [MyExceptionFilter]
#endif
    public class BaseController : Controller
    {
        public ActionResult ResultData(object data, bool isTrue = true, string message = "")
        {
            return Content(JsonConvert.SerializeObject(new { Success = isTrue, IsLogin = true, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }));
        }
    }
}