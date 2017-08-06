using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using IBLL;
using Masuit.Tools;
using Models.Dto;
using Models.Entity;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class RoleController : BaseController
    {
        public IRoleBll RoleBll { get; set; }
        public IUserGroupBll UserGroupBll { get; set; }
        public IUserGroupPermissionBll UserGroupPermissionBll { get; set; }

        public RoleController(IRoleBll roleBll, IUserGroupBll userGroupBll, IUserGroupPermissionBll userGroupPermissionBll)
        {
            RoleBll = roleBll;
            UserGroupBll = userGroupBll;
            UserGroupPermissionBll = userGroupPermissionBll;
        }

        public ActionResult Get(int id)
        {
            Role role = RoleBll.GetById(id);
            RoleOutputDto model = Mapper.Map<RoleOutputDto>(role);
            return ResultData(model);
        }

        public ActionResult GetAllList()
        {
            IQueryable<Role> roles = RoleBll.LoadEntitiesNoTracking(r => true);
            IList<RoleOutputDto> list = Mapper.Map<IList<RoleOutputDto>>(roles.ToList());
            return ResultData(list, roles.Any());
        }

        public ActionResult GetPageData(int page = 1, int size = 10)
        {
            IQueryable<Role> roles = RoleBll.LoadPageEntitiesNoTracking(page, size, out int totalCount, r => true, r => r.Id);
            PageDataViewModel model = new PageDataViewModel() { Data = Mapper.Map<IList<RoleOutputDto>>(roles.ToList()), PageIndex = page, PageSize = size, TotalPage = Math.Ceiling(totalCount.To<double>() / size.To<double>()).ToInt32(), TotalCount = totalCount };
            return ResultData(model, roles.Any());
        }

        public ActionResult Add(RoleInputDto model)
        {
            bool exist = RoleBll.RoleNameExist(model.RoleName);
            if (!exist)
            {
                RoleBll.AddEntitySaved(Mapper.Map<Role>(model));
                return ResultData(model);
            }
            return ResultData(null, exist, $"{model.RoleName}已经存在！");
        }

        public ActionResult Update(RoleInputDto model)
        {
            Role role = RoleBll.GetById(model.Id);
            if (role != null)
            {
                role.RoleName = model.RoleName;
                role.Description = model.Description;
                bool saved = RoleBll.UpdateEntitySaved(role);
                return ResultData(model, saved, saved ? "修改成功！" : "修改失败！");
            }
            return ResultData(null, false, "修改的内容不存在！");
        }

        public ActionResult Delete(int id)
        {
            bool b = RoleBll.DeleteById(id);
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        public ActionResult Deletes(string id)
        {
            string[] ids = id.Split(',');
            IQueryable<Role> roles = RoleBll.LoadEntities(r => ids.Contains(r.Id.ToString()));
            bool b = RoleBll.DeleteEntitiesSaved(roles);
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        public ActionResult AddUserGroup(int id, int rid)
        {
            Task<int> task = UserGroupPermissionBll.AddEntitySavedAsync(new UserGroupPermission() { UserGroupId = id, RoleId = rid, HasPermission = true });
            UserGroup @group = UserGroupBll.GetById(id);
            Role role = RoleBll.GetById(rid);
            bool saved = task.Result > 0;
            return ResultData(null, saved, saved ? $"成功为用户组{group.GroupName}分配角色{role.RoleName}！" : "分配角色失败！");
        }

        public ActionResult RemoveUserGroup(int id, int rid)
        {
            List<UserGroupPermission> list = UserGroupPermissionBll.LoadEntities(p => p.UserGroupId.Equals(id) && p.RoleId.Equals(rid)).ToList();
            Task<int> task = UserGroupPermissionBll.DeleteEntitiesSavedAsync(list);
            UserGroup @group = UserGroupBll.GetById(id);
            Role role = RoleBll.GetById(rid);
            bool saved = task.Result > 0;
            return ResultData(null, saved, saved ? $"成功将{group.GroupName}的{role.RoleName}角色移除！" : "移除失败！");
        }

        public ActionResult Toggle(int id, int rid, bool allow)
        {
            List<UserGroupPermission> list = UserGroupPermissionBll.LoadEntities(p => p.UserGroupId.Equals(id) && p.RoleId.Equals(rid)).ToList();
            list.ForEach(p => p.HasPermission = allow);
            bool b = UserGroupPermissionBll.UpdateEntities(list);
            return ResultData(null, b, b ? $"状态更新成功！" : "切换失败！");
        }
    }
}