using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Common;
using IBLL;
using Masuit.Tools;
using Models.Dto;
using Models.Entity;
using Newtonsoft.Json;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class UserController : Controller
    {
        public IUserInfoBll UserInfoBll { get; set; }

        public UserController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }

        [HashCheckFilter]
        public ActionResult GetUser(Guid id)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            UserInfoViewModel model = Mapper.Map<UserInfoViewModel>(userInfo);
            return Content(JsonConvert.SerializeObject(model));
        }

        public ActionResult GetPermission()
        {
            UserInfo user = Session["user"] as UserInfo;
            List<Function> list = new List<Function>(); //所有允许的权限
            if (user != null)
            {
                //1.0 用户-角色-权限-功能 主线，权限的优先级最低
                user.Role.ForEach(r => r.Permission.ForEach(p => list.AddRange(p.Function)));

                //2.0 用户-用户组-角色-权限，权限的优先级其次
                user.UserGroup?.ForEach(g => g.UserGroupPermission.ForEach(ugp =>
                {
                    if (ugp.HasPermission)
                    {
                        ugp.Role.Permission.ForEach(p => list.AddRange(p.Function));
                    }
                    else
                    {
                        ugp.Role.Permission.ForEach(p => list = list.Except(p.Function).ToList());
                    }
                }));

                //3.0 用户-权限-功能 临时权限，权限的优先级最高
                user.UserPermission?.ForEach(p =>
                {
                    if (p.HasPermission)
                    {
                        list.AddRange(p.Permission.Function);
                    }
                    else
                    {
                        list = list.Except(p.Permission.Function).ToList();
                    }
                });
            }
            return Content(JsonConvert.SerializeObject(Mapper.Map<IList<FunctionDto>>(list)));
        }
    }
}