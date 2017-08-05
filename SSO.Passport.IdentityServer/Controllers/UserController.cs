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

namespace SSO.Passport.IdentityServer.Controllers
{
    public class UserController : BaseController
    {
        public IUserInfoBll UserInfoBll { get; set; }
        public IUserGroupBll UserGroupBll { get; set; }

        public UserController(IUserInfoBll userInfoBll, IUserGroupBll userGroupBll)
        {
            UserInfoBll = userInfoBll;
            UserGroupBll = userGroupBll;
        }

        public ActionResult GetUser(Guid id)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            UserInfoOutputDto model = Mapper.Map<UserInfoOutputDto>(userInfo);
            return Content(JsonConvert.SerializeObject(model));
        }

        public ActionResult GetAllList()
        {
            IQueryable<UserInfo> userInfos = UserInfoBll.LoadEntitiesNoTracking(u => true);
            IList<UserInfo> list = Mapper.Map<IList<UserInfo>>(userInfos.ToList());
            return ResultData(list, userInfos.Any());
        }

        public ActionResult GetPageData(int page = 1, int size = 10)
        {
            IQueryable<UserInfo> userInfos = UserInfoBll.LoadPageEntitiesNoTracking(page, size, out int totalCount, u => true, u => u.Id, false);
            PageDataViewModel model = new PageDataViewModel()
            {
                Data = Mapper.Map<IList<UserInfo>>(userInfos.ToList()),
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
                PageDataViewModel model = new PageDataViewModel() { Data = Mapper.Map<IList<UserInfo>>(userInfos.ToList()), PageIndex = page, PageSize = size, TotalPage = Math.Ceiling(all.Count.To<double>() / size.To<double>()).ToInt32(), TotalCount = all.Count };
                return ResultData(model);
            }
            return ResultData(null, false, "没有数据");
        }

        public ActionResult Add(UserInfoInputDto model, int? gid)
        {
            if (UserInfoBll.UsernameExist(model.Username))
            {
                return ResultData(model, false, $"用户名{model.Username}已经存在！");
            }
            else if (UserInfoBll.EmailExist(model.Email))
            {
                return ResultData(model, false, $"邮箱{model.Email}已经存在！");
            }
            else if (UserInfoBll.PhoneExist(model.PhoneNumber))
            {
                return ResultData(model, false, $"电话号码{model.PhoneNumber}已经存在！");
            }
            UserInfo userInfo = UserInfoBll.Register(Mapper.Map<UserInfo>(model));
            if (gid != null)
            {
                UserGroup @group = UserGroupBll.GetById(gid);
                group.UserInfo.Add(userInfo);
                UserGroupBll.SaveChanges();
            }
            return ResultData(Mapper.Map<UserInfoOutputDto>(userInfo));
        }

        public ActionResult Update(UserInfoInputDto model)
        {
            if (UserInfoBll.UsernameExist(model.Username))
            {
                return ResultData(model, false, $"用户名{model.Username}已经存在！");
            }
            else if (UserInfoBll.EmailExist(model.Email))
            {
                return ResultData(model, false, $"邮箱{model.Email}已经存在！");
            }
            else if (UserInfoBll.PhoneExist(model.PhoneNumber))
            {
                return ResultData(model, false, $"电话号码{model.PhoneNumber}已经存在！");
            }
            UserInfo userInfo = UserInfoBll.GetByUsername(model.Username);
            userInfo.Username = model.Username;
            userInfo.Email = model.Email;
            userInfo.PhoneNumber = model.PhoneNumber;
            bool b = UserInfoBll.SaveChanges() > 0;
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
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }
        public ActionResult Deletes(string id)
        {
            string[] ids = id.Split(',');
            IQueryable<UserInfo> userInfos = UserInfoBll.LoadEntities(u => ids.Contains(u.Id.ToString()));
            bool b = UserInfoBll.DeleteEntitiesSaved(userInfos);
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

    }
}