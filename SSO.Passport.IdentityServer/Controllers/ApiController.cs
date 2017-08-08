using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public ActionResult ResultData(object data, bool isTrue = true, string message = "")
        {
            return ResultData(JsonConvert.SerializeObject(new { Success = isTrue, Message = message, Data = data }, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore }));
        }

        public ActionResult GetUser(Guid id)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            UserInfoLoginModel model = userInfo.MapTo<UserInfoLoginModel>();
            return ResultData(JsonConvert.SerializeObject(model));
        }


        public ActionResult GetPermission(Guid id)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            IEnumerable<Function> list = UserInfoBll.GetPermissionList(userInfo);
            return ResultData(JsonConvert.SerializeObject(Mapper.Map<IList<FunctionOutputDto>>(list.ToList())));
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
            Match match = model.Email.MatchEmail(out bool flag);
            if (!match.Success)
            {
                return ResultData(model, false, $"邮箱格式不正确！");
            }
            match = model.PhoneNumber.MatchPhoneNumber(out flag);
            if (!match.Success)
            {
                return ResultData(model, false, $"手机号码格式不正确！");
            }
            UserInfo userInfo = UserInfoBll.Register(Mapper.Map<UserInfo>(model));
            return ResultData(Mapper.Map<UserInfoOutputDto>(userInfo));
        }
    }
}