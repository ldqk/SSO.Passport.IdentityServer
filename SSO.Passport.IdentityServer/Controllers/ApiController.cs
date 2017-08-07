using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using IBLL;
using Masuit.Tools;
using Models.Dto;
using Models.Entity;
using Models.Enum;
using Newtonsoft.Json;
using SSO.Core.Client;

namespace SSO.Passport.IdentityServer.Controllers
{
#if !DEBUG
    [Authority(Code = AuthCodeEnum.HashCheck)] 
#endif
    public class ApiController : Controller
    {
        public IUserInfoBll UserInfoBll { get; set; }

        public ApiController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }

        public ActionResult GetUser(Guid id)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            UserInfoLoginModel model = userInfo.MapTo<UserInfoLoginModel>();
            return Content(JsonConvert.SerializeObject(model));
        }


        public ActionResult GetPermission(Guid id)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            IEnumerable<Function> list = UserInfoBll.GetPermissionList(userInfo);
            return Content(JsonConvert.SerializeObject(Mapper.Map<IList<FunctionOutputDto>>(list.ToList())));
        }
        public ActionResult GetMenu(Guid id)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            IEnumerable<Function> list = UserInfoBll.GetPermissionList(userInfo, FunctionType.Menu);
            return Content(JsonConvert.SerializeObject(Mapper.Map<IList<FunctionOutputDto>>(list.ToList())));
        }

        public ActionResult ChangePassword(Guid id, string old, string pwd, string pwd2)
        {
            if (pwd.Equals(pwd2))
            {
                bool b = UserInfoBll.ChangePassword(id, old, pwd);
                return Content(b ? $"密码修改成功，新密码为：{pwd}！" : "密码修改失败！");
            }
            return Content("两次输入的密码不一致！");
        }
    }
}