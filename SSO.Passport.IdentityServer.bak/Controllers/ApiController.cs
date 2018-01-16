using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using IBLL;
using Masuit.Tools;
using Models.Dto;
using Models.Entity;
using Newtonsoft.Json;
using SSO.Core.Client;
using SSO.Passport.IdentityServer.Models;

namespace SSO.Passport.IdentityServer.Controllers
{
#if !DEBUG
    //[Authority(Code = AuthCodeEnum.HashCheck)] 
#endif
    [MyExceptionFilter]

    public class ApiController : Controller
    {
        public IUserInfoBll UserInfoBll { get; set; }

        public ApiController(IUserInfoBll userInfoBll)
        {
            UserInfoBll = userInfoBll;
        }

        public ActionResult ResultData(object data, bool isTrue = true, string message = "")
        {
            return Content(JsonConvert.SerializeObject(new { Success = isTrue, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }));
        }


        public ActionResult GetUser(Guid id)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            UserInfoLoginModel model = userInfo.MapTo<UserInfoLoginModel>();
            return ResultData(model);
        }


        public ActionResult GetPermission(Guid id)
        {
            IEnumerable<Control> list = UserInfoBll.GetPermissionList(id);
            return ResultData(Mapper.Map<IList<ControlOutputDto>>(list.ToList()));
        }

        public ActionResult ChangePassword(Guid id, string old, string pwd, string pwd2)
        {
            if (pwd.Equals(pwd2))
            {
                bool b = UserInfoBll.ChangePassword(id, old, pwd);
                return ResultData(null, b, b ? $"密码修改成功，新密码为：{pwd}！" : "密码修改失败！");
            }
            return ResultData(null, false, "两次输入的密码不一致！");
        }

        public ActionResult Register(UserInfoInputDto model, string valid)
        {
            if (model.Email.Trim().IsNullOrEmpty())
            {
                return ResultData(model, false, $"邮箱不能为空！");
            }
            if (model.Username.Trim().IsNullOrEmpty())
            {
                return ResultData(model, false, $"用户名不能为空！");
            }
            if (model.Password.Trim().IsNullOrEmpty())
            {
                return ResultData(model, false, $"密码不能为空！");
            }
            if (UserInfoBll.UsernameExist(model.Username))
            {
                return ResultData(model, false, $"用户名{model.Username}已经存在！");
            }
            if (UserInfoBll.EmailExist(model.Email))
            {
                return ResultData(model, false, $"邮箱{model.Email}已经存在！");
            }
            if (UserInfoBll.PhoneExist(model.PhoneNumber))
            {
                return ResultData(model, false, $"电话号码{model.PhoneNumber}已经存在！");
            }
            model.Email.MatchEmail(out bool flag);
            if (!flag)
            {
                return ResultData(model, false, $"邮箱格式不正确！");
            }
            model.PhoneNumber.MatchPhoneNumber(out flag);
            if (!flag)
            {
                return ResultData(model, false, $"手机号码格式不正确！");
            }
            UserInfo userInfo = UserInfoBll.Register(Mapper.Map<UserInfo>(model));
            return ResultData(Mapper.Map<UserInfoOutputDto>(userInfo));
        }

        [Authority(Code = AuthCodeEnum.Public)]
        public ActionResult Login(string name, string pwd)
        {
            UserInfo userInfo = UserInfoBll.Login(name, pwd);
            if (userInfo != null)
            {
                SessionHelper.Set(userInfo.Id.ToString(), userInfo.MapTo<UserInfoLoginModel>());
                return ResultData(new { user = userInfo.MapTo<UserInfoLoginModel>(), menus = UserInfoBll.GetPermissionList(userInfo.Id).Select(c => new { id = c.Id, name = c.Name, url = c.Controller + c.Action, show = true }) });
            }
            return ResultData(null, false, "用户名或密码错误！");
        }

        public ActionResult ChangeUsername(Guid id, string username)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            if (!username.Equals(userInfo.Username) && UserInfoBll.UsernameExist(username))
            {
                return ResultData(null, false, $"用户名{username}已经存在，请尝试更换其他用户名！");
            }
            userInfo.Username = username;
            bool b = UserInfoBll.UpdateEntitySaved(userInfo);
            return ResultData(Mapper.Map<UserInfoOutputDto>(userInfo), b, b ? $"用户名修改成功，新用户名为{username}。" : "用户名修改失败！");
        }
    }
}