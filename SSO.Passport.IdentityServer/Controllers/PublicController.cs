using System.Web.Http;
using Masuit.Tools.Hardware;

namespace SSO.Passport.IdentityServer.Controllers
{
    /// <summary>
    /// 公共api
    /// </summary>
    [Route("api/[action]")]
    public class PublicController : ApiController
    {
        /// <summary>
        /// api测试
        /// </summary>
        [HttpGet]
        public string Get()
        {
            return SystemInfo.GetProcessorData();
        }
    }
}