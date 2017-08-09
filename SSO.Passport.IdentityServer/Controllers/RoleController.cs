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
using Models.ViewModel;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class RoleController : BaseController
    {
        public IRoleBll RoleBll { get; set; }
        public IUserGroupBll UserGroupBll { get; set; }
        public IUserGroupPermissionBll UserGroupPermissionBll { get; set; }

        public RoleController(IRoleBll roleBll, IUserGroupBll userGroupBll, IUserGroupPermissionBll userGroupPermissionBll, IUserInfoBll userInfoBll)
        {
            RoleBll = roleBll;
            UserGroupBll = userGroupBll;
            UserGroupPermissionBll = userGroupPermissionBll;
            UserInfoBll = userInfoBll;
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
            bool b = RoleBll.DeleteEntity(r => r.Id == id) > 0;
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        public ActionResult Deletes(string id)
        {
            string[] ids = id.Split(',');
            bool b = RoleBll.DeleteEntity(r => ids.Contains(r.Id.ToString())) > 0;
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        public ActionResult AddUserGroup(int id, int rid)
        {
            UserGroupPermission permission = UserGroupPermissionBll.AddEntitySaved(new UserGroupPermission() { UserGroupId = id, RoleId = rid, HasPermission = true });
            UserGroup @group = UserGroupBll.GetById(id);
            Role role = RoleBll.GetById(rid);
            bool saved = permission != null;
            return ResultData(null, saved, saved ? $"成功为用户组{group.GroupName}分配角色{role.RoleName}！" : "分配角色失败！");
        }

        public ActionResult RemoveUserGroup(int id, int rid)
        {
            List<UserGroupPermission> list = UserGroupPermissionBll.LoadEntities(p => p.UserGroupId.Equals(id) && p.RoleId.Equals(rid)).ToList();
            bool res = UserGroupPermissionBll.DeleteEntitiesSaved(list);
            UserGroup @group = UserGroupBll.GetById(id);
            Role role = RoleBll.GetById(rid);
            return ResultData(null, res, res ? $"成功将{group.GroupName}的{role.RoleName}角色移除！" : "移除失败！");
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