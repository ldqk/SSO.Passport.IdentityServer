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
using Newtonsoft.Json;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class UserController : BaseController
    {
        public IUserGroupBll UserGroupBll { get; set; }

        public UserController(IUserInfoBll userInfoBll, IUserGroupBll userGroupBll)
        {
            UserInfoBll = userInfoBll;
            UserGroupBll = userGroupBll;
        }

        public ActionResult Get(Guid id)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            UserInfoOutputDto model = Mapper.Map<UserInfoOutputDto>(userInfo);
            return Content(JsonConvert.SerializeObject(model));
        }

        public ActionResult GetAllList()
        {
            IQueryable<UserInfo> userInfos = UserInfoBll.LoadEntitiesNoTracking(u => true);
            IList<UserInfoOutputDto> list = Mapper.Map<IList<UserInfoOutputDto>>(userInfos.ToList());
            return ResultData(list, userInfos.Any());
        }

        public ActionResult GetPageData(int page = 1, int size = 10)
        {
            IQueryable<UserInfo> userInfos = UserInfoBll.LoadPageEntitiesNoTracking(page, size, out int totalCount, u => true, u => u.Id, false);
            PageDataViewModel model = new PageDataViewModel()
            {
                Data = Mapper.Map<IList<UserInfoOutputDto>>(userInfos.ToList()),
                PageIndex = page,
                PageSize = size,
                TotalPage = Math.Ceiling(totalCount.To<double>() / size.To<double>()).ToInt32(),
                TotalCount = totalCount
            };
            return ResultData(model, userInfos.Any());
        }

        public ActionResult GetPageDataByGroup(int id, int page = 1, int size = 10)
        {
            ICollection<UserInfo> all = UserGroupBll.GetById(id).UserInfo;
            if (all.Any())
            {
                IEnumerable<UserInfo> userInfos = all.OrderByDescending(u => u.Id).Skip(size * (page - 1)).Take(size);
                PageDataViewModel model = new PageDataViewModel() { Data = Mapper.Map<IList<UserInfoOutputDto>>(userInfos.ToList()), PageIndex = page, PageSize = size, TotalPage = Math.Ceiling(all.Count.To<double>() / size.To<double>()).ToInt32(), TotalCount = all.Count };
                return ResultData(model);
            }
            return ResultData(null, false, "没有数据");
        }

        public ActionResult Add(UserInfoInputDto model, int? gid)
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
            if (gid != null)
            {
                UserGroup @group = UserGroupBll.GetById(gid);
                userInfo.UserGroup.Add(group);
                UserInfoBll.UpdateEntitySaved(userInfo);
            }
            return ResultData(Mapper.Map<UserInfoOutputDto>(userInfo));
        }

        public ActionResult Update(UserInfoInputDto model, int? gid)
        {
            UserInfo user = UserInfoBll.GetById(model.Id);
            IQueryable<UserInfo> all = UserInfoBll.LoadEntities(u => !u.Username.Equals(user.Username) && !u.Email.Equals(user.Email) && !u.PhoneNumber.Equals(user.PhoneNumber));
            if (all.Any(u => u.Username.Equals(model.Username)))
            {
                return ResultData(model, false, $"用户名{model.Username}已经存在！");
            }
            if (all.Any(u => u.Email.Equals(model.Email)))
            {
                return ResultData(model, false, $"邮箱{model.Email}已经存在！");
            }
            if (all.Any(u => u.PhoneNumber.Equals(model.PhoneNumber)))
            {
                return ResultData(model, false, $"电话号码{model.PhoneNumber}已经存在！");
            }
            user.Username = model.Username;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            bool b = UserInfoBll.UpdateEntitySaved(user);
            return ResultData(Mapper.Map<UserInfoOutputDto>(model), b, b ? "修改成功！" : "修改失败！");
        }

        public ActionResult ResetPassword(string name, string newPwd)
        {
            bool b = UserInfoBll.ResetPassword(name, newPwd);
            return ResultData(null, b, b ? $"密码重置成功，新密码为：{newPwd}！" : "密码重置失败！");
        }

        public ActionResult Delete(Guid id)
        {
            bool b = UserInfoBll.DeleteById(id);
            UserInfoBll.SaveChanges();
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }
        public ActionResult Deletes(string id)
        {
            string[] ids = id.Split(',');
            bool b = UserInfoBll.DeleteEntity(u => ids.Contains(u.Id.ToString())) > 0;
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

    }
}