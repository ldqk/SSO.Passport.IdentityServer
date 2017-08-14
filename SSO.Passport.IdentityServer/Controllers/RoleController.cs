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

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Get(int id)
        {
            Role role = RoleBll.GetById(id);
            return View(role);
        }

        public ActionResult GetAllList()
        {
            IQueryable<Role> roles = RoleBll.LoadEntities(r => true);
            IList<RoleOutputDto> list = Mapper.Map<IList<RoleOutputDto>>(roles.ToList());
            return ResultData(list, roles.Any());
        }

        public ActionResult GetPageData(int start = 1, int length = 10)
        {
            var search = Request["search[value]"];
            bool b = search.IsNullOrEmpty();
            var page = start / length + 1;
            IQueryable<Role> roles = RoleBll.LoadPageEntities(page, length, out int totalCount, r => b || r.RoleName.Contains(search), r => r.Id);
            DataTableViewModel model = new DataTableViewModel() { data = Mapper.Map<IList<RoleOutputDto>>(roles.ToList()), recordsFiltered = totalCount, recordsTotal = totalCount };
            return Content(model.ToJsonString());
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

        public ActionResult Toggle(string gid, string rid, string has)
        {
            string[] gids = gid.Trim(',').Split(',');
            string[] rids = rid.Trim(',').Split(',');
            string[] allows = has.Trim(',').Split(',');
            IList<UserGroupPermission> list = new List<UserGroupPermission>();
            for (var i = 0; i < gids.Length; i++)
            {
                var groupid = gids[i].ToInt32();
                var rsid = rids[i].ToInt32();
                var hasps = allows[i].To<bool>();
                UserGroupPermissionBll.DeleteEntity(p => p.UserGroupId.Equals(groupid) && p.RoleId.Equals(rsid));
                list.Add(new UserGroupPermission() { HasPermission = hasps, UserGroupId = groupid, RoleId = rsid });
            }
            IEnumerable<UserGroupPermission> ups = UserGroupPermissionBll.AddEntities(list);
            bool b = ups.Any();
            return ResultData(null, b, b ? $"状态更新成功！" : "切换失败！");
        }


        public ActionResult UserNoHasRole(Guid id)
        {
            IEnumerable<Role> roles = RoleBll.LoadEntities(r => true).ToList().Except(UserInfoBll.GetById(id).Role.ToList());
            return ResultData(Mapper.Map<IList<RoleOutputDto>>(roles.ToList()));
        }
        public ActionResult UserRoleList(Guid id)
        {
            return ResultData(Mapper.Map<IList<RoleOutputDto>>(UserInfoBll.GetById(id).Role.ToList()));
        }

        public ActionResult UpdateUserRole(Guid id, string rids)
        {
            string[] strs = rids.Split(',');
            IQueryable<Role> roles = RoleBll.LoadEntities(r => strs.Contains(r.Id.ToString()));
            UserInfo userInfo = UserInfoBll.GetById(id);
            userInfo.Role.Clear();
            roles.ToList().ForEach(r => userInfo.Role.Add(r));
            bool b = UserInfoBll.SaveChanges() > 0;
            return ResultData(null, b, b ? "角色分配成功！" : "角色分配失败！");
        }


        public ActionResult NoHasGroup(int id)
        {
            IList<Role> list = new List<Role>();
            UserGroupBll.GetById(id).UserGroupPermission.ToList().ForEach(p => list.Add(p.Role));
            IEnumerable<Role> roles = RoleBll.LoadEntities(r => true).ToList().Except(list);
            return ResultData(Mapper.Map<IList<RoleOutputDto>>(roles.ToList()));
        }

        public ActionResult GroupList(int id)
        {
            IList<Role> list = new List<Role>();
            UserGroupBll.GetById(id).UserGroupPermission.ToList().ForEach(p => list.Add(p.Role));
            return ResultData(Mapper.Map<IList<RoleOutputDto>>(list));
        }

        public ActionResult UpdateGroup(int id, string rids)
        {
            string[] strs = rids.Split(',');
            IQueryable<Role> roles = RoleBll.LoadEntities(r => strs.Contains(r.Id.ToString()));
            UserGroup @group = UserGroupBll.GetById(id);
            bool b = UserGroupPermissionBll.DeleteEntity(u => u.UserGroupId.Equals(@group.Id)) > 0;
            IList<UserGroupPermission> list = new List<UserGroupPermission>();
            roles.ToList().ForEach(p => list.Add(new UserGroupPermission() { UserGroupId = @group.Id, RoleId = p.Id, HasPermission = true }));
            UserGroupPermissionBll.AddEntities(list);
            return ResultData(null, b, b ? "角色分配成功！" : "角色分配失败！");
        }

        public ActionResult UserGroup(int id)
        {
            Role role = RoleBll.GetById(id);
            return View(role);
        }

        public ActionResult Permission(int id)
        {
            Role role = RoleBll.GetById(id);
            return View(role);
        }

        public ActionResult NoHasUserGroup(int id)
        {
            IList<UserGroup> list = new List<UserGroup>();
            RoleBll.GetById(id).UserGroupPermission.ToList().ForEach(p => list.Add(p.UserGroup));
            IEnumerable<UserGroup> groups = UserGroupBll.LoadEntities(r => true).ToList().Except(list);
            return ResultData(Mapper.Map<IList<UserGroupOutputDto>>(groups.ToList()));
        }

        public ActionResult UserGroupList(int id)
        {
            IList<UserGroup> list = new List<UserGroup>();
            RoleBll.GetById(id).UserGroupPermission.ToList().ForEach(p => list.Add(p.UserGroup));
            return ResultData(Mapper.Map<IList<UserGroupOutputDto>>(list));
        }

        public ActionResult UpdateRole(int id, string gids)
        {
            string[] strs = gids.Split(',');
            IQueryable<UserGroup> groups = UserGroupBll.LoadEntities(r => strs.Contains(r.Id.ToString()));
            Role role = RoleBll.GetById(id);
            bool b = UserGroupPermissionBll.DeleteEntity(u => u.RoleId.Equals(role.Id)) > 0;
            IList<UserGroupPermission> list = new List<UserGroupPermission>();
            groups.ToList().ForEach(p => list.Add(new UserGroupPermission() { RoleId = role.Id, UserGroupId = p.Id, HasPermission = true }));
            UserGroupPermissionBll.AddEntities(list);
            return ResultData(null, b, b ? "角色分配成功！" : "角色分配失败！");
        }
    }
}