using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using Common;
using IBLL;
using Models.Dto;
using Models.Entity;

namespace SSO.Passport.IdentityServer.Controllers
{
    /// <summary>
    /// 公共api
    /// </summary>
    [System.Web.Http.Route("api/{action}")]
    public class PublicController : ApiController
    {
        public IUserInfoBll UserInfoBll { get; set; }

        public PublicController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }

        /// <summary>
        /// 获取访问控制权限
        /// </summary>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        [HttpPost, Route("api/acl")]
        public List<ControlOutputDto> AccessControls(string accessKey)
        {
            UserInfo userInfo = UserInfoBll.GetFirstEntity(u => u.AccessKey.Equals(accessKey));
            List<ControlOutputDto> controls = UserInfoBll.GetAccessControls(ConfigurationManager.AppSettings["AppId"], userInfo.Id);
            return controls;
        }

        /// <summary>
        /// 获取菜单权限
        /// </summary>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        [HttpPost]
        public List<MenuOutputDto> Menus(string accessKey)
        {
            UserInfo userInfo = UserInfoBll.GetFirstEntity(u => u.AccessKey.Equals(accessKey));
            List<MenuOutputDto> menus = UserInfoBll.GetMenus(ConfigurationManager.AppSettings["AppId"], userInfo.Id);
            return menus;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        [HttpPost]
        public UserInfoDto User(string accessKey)
        {
            return UserInfoBll.GetFirstEntity(u => u.AccessKey.Equals(accessKey)).Mapper<UserInfoDto>();
        }
    }
}