using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using IBLL;
using Masuit.Tools;
using Models.Dto;
using Models.Entity;
using Models.ViewModel;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class UserGroupController : BaseController
    {
        public IUserGroupBll UserGroupBll { get; set; }

        public UserGroupController(IUserGroupBll userGroupBll, IUserInfoBll userInfoBll)
        {
            UserGroupBll = userGroupBll;
            UserInfoBll = userInfoBll;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAllList()
        {
            IQueryable<UserGroup> groups = UserGroupBll.LoadEntitiesNoTracking(r => true);
            IList<UserGroupOutputDto> list = Mapper.Map<IList<UserGroupOutputDto>>(groups.ToList());
            return ResultData(list, groups.Any());
        }

        public ActionResult GetPageData(int start = 1, int length = 10)
        {
            var search = Request["search[value]"];
            bool b = search.IsNullOrEmpty();
            var page = start / length + 1;
            IQueryable<UserGroup> groups = UserGroupBll.LoadPageEntitiesNoTracking(page, length, out int totalCount, r => b || r.GroupName.Contains(search), r => r.Id);
            DataTableViewModel model = new DataTableViewModel() { data = Mapper.Map<IList<UserGroupOutputDto>>(groups.ToList()), recordsFiltered = totalCount, recordsTotal = totalCount };
            return Content(model.ToJsonString());
        }

        [HttpPost]
        public ActionResult Add(UserGroupInputDto model)
        {
            bool exist = UserGroupBll.GroupNameExist(model.GroupName);
            if (!exist)
            {
                UserGroupBll.AddEntitySaved(Mapper.Map<UserGroup>(model));
                return ResultData(model);
            }
            return ResultData(null, !exist, $"{model.GroupName}已经存在！");
        }

        public ActionResult Update(UserGroupInputDto model)
        {
            UserGroup @group = UserGroupBll.GetById(model.Id);
            if (group != null)
            {
                group.GroupName = model.GroupName;
                group.ParentId = model.ParentId;
                bool saved = UserGroupBll.UpdateEntitySaved(group);
                return ResultData(model, saved, saved ? "修改成功！" : "修改失败！");
            }
            return ResultData(null, false, "修改的内容不存在！");
        }

        public ActionResult Delete(int id)
        {
            bool b = UserGroupBll.DeleteEntity(g => g.Id == id) > 0;
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        public ActionResult Deletes(string id)
        {
            string[] ids = id.Split(',');
            bool b = UserGroupBll.DeleteEntity(g => ids.Contains(g.Id.ToString())) > 0;
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        public ActionResult AddUser(Guid id, string gname)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            UserGroup @group = UserGroupBll.GetGroupByName(gname);
            group.UserInfo.Add(userInfo);
            bool saved = UserGroupBll.UpdateEntitySaved(@group);
            return ResultData(null, saved, saved ? $"成功将{userInfo.Username}添加到用户组{group.GroupName}！" : "添加失败！");
        }

        public ActionResult RemoveUser(Guid id, string gname)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            UserGroup @group = UserGroupBll.GetGroupByName(gname);
            group.UserInfo.Remove(userInfo);
            bool saved = UserGroupBll.UpdateEntitySaved(@group);
            return ResultData(null, saved, saved ? $"成功将{userInfo.Username}从用户组{group.GroupName}移除！" : "移除失败！");
        }

        public ActionResult MoveUser(Guid id, string from, string to)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            UserGroup f = UserGroupBll.GetGroupByName(from);
            UserGroup t = UserGroupBll.GetGroupByName(to);
            f.UserInfo.Remove(userInfo);
            t.UserInfo.Add(userInfo);
            UserGroupBll.UpdateEntity(f);
            UserGroupBll.UpdateEntity(t);
            bool saved = UserGroupBll.SaveChanges() > 0;
            return ResultData(null, saved, saved ? $"成功将{userInfo.Username}从用户组{f.GroupName}移动到{t.GroupName}！" : "移动失败！");
        }
    }
}