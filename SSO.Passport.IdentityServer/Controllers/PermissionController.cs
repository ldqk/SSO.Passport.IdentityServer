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
using Models.Enum;
using Models.ViewModel;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class PermissionController : BaseController
    {
        public IPermissionBll PermissionBll { get; set; }
        public IUserPermissionBll UserPermissionBll { get; set; }
        public IRoleBll RoleBll { get; set; }

        public PermissionController(IPermissionBll permissionBll, IUserPermissionBll userPermissionBll, IUserInfoBll userInfoBll, IRoleBll roleBll)
        {
            PermissionBll = permissionBll;
            UserPermissionBll = userPermissionBll;
            UserInfoBll = userInfoBll;
            RoleBll = roleBll;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get(int id)
        {
            return View(PermissionBll.GetById(id));
        }

        public ActionResult GetAllList()
        {
            IQueryable<Permission> permissions = PermissionBll.LoadEntitiesNoTracking(r => true);
            IList<PermissionOutputDto> list = Mapper.Map<IList<PermissionOutputDto>>(permissions.ToList());
            return ResultData(list, permissions.Any());
        }

        public ActionResult GetPageData(int start = 1, int length = 10)
        {
            var search = Request["search[value]"];
            bool b = search.IsNullOrEmpty();
            var page = start / length + 1;
            IQueryable<Permission> permissions = PermissionBll.LoadPageEntitiesNoTracking(page, length, out int totalCount, r => b || r.PermissionName.Contains(search), r => r.Id);
            DataTableViewModel model = new DataTableViewModel() { data = Mapper.Map<IList<PermissionOutputDto>>(permissions.ToList()), recordsFiltered = totalCount, recordsTotal = totalCount };
            return Content(model.ToJsonString());
        }

        public ActionResult Add(PermissionInputDto model)
        {
            bool exist = PermissionBll.PermissionNameExist(model.PermissionName);
            if (!exist)
            {
                PermissionBll.AddEntitySaved(Mapper.Map<Permission>(model));
                return ResultData(model);
            }
            return ResultData(null, exist, $"{model.PermissionName}已经存在！");
        }

        public ActionResult Update(PermissionInputDto model)
        {
            Permission permission = PermissionBll.GetById(model.Id);
            if (permission != null)
            {
                permission.PermissionName = model.PermissionName;
                permission.Description = model.Description;
                bool saved = PermissionBll.UpdateEntitySaved(permission);
                return ResultData(model, saved, saved ? "修改成功！" : "修改失败！");
            }
            return ResultData(null, false, "修改的内容不存在！");
        }

        public ActionResult Delete(int id)
        {
            bool b = PermissionBll.DeleteEntity(p => p.Id == id) > 0;
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        public ActionResult Deletes(string id)
        {
            string[] ids = id.Split(',');
            bool b = PermissionBll.DeleteEntity(r => ids.Contains(r.Id.ToString())) > 0;
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        public ActionResult AddUser(Guid id, int pid)
        {
            Task<int> task = UserPermissionBll.AddEntitySavedAsync(new UserPermission() { UserInfoId = id, PermissionId = pid, HasPermission = true });
            Task<UserInfo> userInfo = UserInfoBll.GetByIdAsync(id);
            Task<Permission> permission = PermissionBll.GetByIdAsync(pid);
            bool saved = task.Result > 0;
            return ResultData(null, saved, saved ? $"成功为用户{userInfo.Result.Username}分配临时权限{permission.Result.PermissionName}！" : "分配权限失败！");
        }

        public ActionResult RemoveUser(Guid id, int pid)
        {
            List<UserPermission> list = UserPermissionBll.LoadEntities(p => p.UserInfoId.Equals(id) && p.PermissionId.Equals(pid)).ToList();
            Task<int> task = UserPermissionBll.DeleteEntitiesSavedAsync(list);
            UserInfo userInfo = UserInfoBll.GetById(id);
            Permission permission = PermissionBll.GetById(pid);
            bool saved = task.Result > 0;
            return ResultData(null, saved, saved ? $"成功将{userInfo.Username}的{permission.PermissionName}权限移除！" : "移除失败！");
        }

        public ActionResult Toggle(string uid, string pid, string has)
        {
            string[] uids = uid.Trim(',').Split(',');
            string[] pids = pid.Trim(',').Split(',');
            string[] allows = has.Trim(',').Split(',');
            IList<UserPermission> list = new List<UserPermission>();
            for (var i = 0; i < uids.Length; i++)
            {
                var userId = Guid.Parse(uids[i]);
                var psid = pids[i].ToInt32();
                var hasps = allows[i].To<bool>();
                UserPermissionBll.DeleteEntity(p => p.UserInfoId.Equals(userId) && p.PermissionId.Equals(psid));
                list.Add(new UserPermission() { HasPermission = hasps, UserInfoId = userId, PermissionId = psid });
            }
            IEnumerable<UserPermission> ups = UserPermissionBll.AddEntities(list);
            bool b = ups.Any();
            return ResultData(null, b, b ? $"状态更新成功！" : "切换失败！");
        }

        public ActionResult AddRole(int id, int pid)
        {
            Role role = RoleBll.GetById(id);
            Permission permission = PermissionBll.GetById(pid);
            permission.Role.Add(role);
            bool saved = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, saved, saved ? $"成功为{role.RoleName}分配{permission.PermissionName}权限！" : "权限分配失败！");
        }


        public ActionResult NoHasPermission(Guid id)
        {
            IList<Permission> list = new List<Permission>();
            UserInfoBll.GetById(id).UserPermission.ToList().ForEach(p => list.Add(p.Permission));
            IEnumerable<Permission> permissions = PermissionBll.LoadEntities(r => true).ToList().Except(list);
            return ResultData(Mapper.Map<IList<PermissionOutputDto>>(permissions.ToList()));
        }

        public ActionResult UserPermissionList(Guid id)
        {
            IList<Permission> list = new List<Permission>();
            UserInfoBll.GetById(id).UserPermission.ToList().ForEach(p => list.Add(p.Permission));
            return ResultData(Mapper.Map<IList<PermissionOutputDto>>(list));

        }

        public ActionResult UpdateUserPermission(Guid id, string pids)
        {
            string[] strs = pids.Split(',');
            IQueryable<Permission> permissions = PermissionBll.LoadEntities(r => strs.Contains(r.Id.ToString()));
            UserInfo userInfo = UserInfoBll.GetById(id);
            bool b = UserPermissionBll.DeleteEntity(u => u.UserInfoId.Equals(userInfo.Id)) > 0;
            IList<UserPermission> list = new List<UserPermission>();
            permissions.ToList().ForEach(p => list.Add(new UserPermission() { UserInfoId = userInfo.Id, PermissionId = p.Id, HasPermission = true }));
            UserPermissionBll.AddEntities(list);
            return ResultData(null, b, b ? "临时权限分配成功！" : "临时权限分配失败！");
        }

        public ActionResult RemoveRole(int id, int pid)
        {
            Role role = RoleBll.GetById(id);
            Permission permission = PermissionBll.GetById(pid);
            permission.Role.Remove(role);
            bool saved = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, saved, saved ? $"成功将{role.RoleName}的{permission.PermissionName}权限移除！" : "权限移除失败！");
        }

        public ActionResult MoveRole(int id, int from, int to)
        {
            Role role = RoleBll.GetById(id);
            Permission f = PermissionBll.GetById(from);
            Permission t = PermissionBll.GetById(to);
            f.Role.Remove(role);
            t.Role.Add(role);
            PermissionBll.UpdateEntity(f);
            PermissionBll.UpdateEntity(t);
            bool saved = PermissionBll.SaveChanges() > 0;
            return ResultData(null, saved, saved ? $"成功将{role.RoleName}从权限{f.PermissionName}移动到{t.PermissionName}！" : "移动失败！");
        }


        public ActionResult RoleNoHasPermission(int id)
        {
            IEnumerable<Permission> permissions = PermissionBll.LoadEntities(r => true).ToList().Except(RoleBll.GetById(id).Permission.ToList());
            return ResultData(Mapper.Map<IList<PermissionOutputDto>>(permissions.ToList()));
        }

        public ActionResult RolePermissionList(int id)
        {
            return ResultData(Mapper.Map<IList<PermissionOutputDto>>(RoleBll.GetById(id).Permission.ToList()));
        }

        public ActionResult UpdateRolePermission(int id, string pids)
        {
            string[] strs = pids.Split(',');
            IQueryable<Permission> permissions = PermissionBll.LoadEntities(r => strs.Contains(r.Id.ToString()));
            Role role = RoleBll.GetById(id);
            role.Permission.Clear();
            permissions.ToList().ForEach(r => role.Permission.Add(r));
            RoleBll.UpdateEntity(role);
            bool b = RoleBll.SaveChanges() > 0;
            return ResultData(null, b, b ? "权限分配成功！" : "权限分配失败！");
        }

        public ActionResult User(int id)
        {
            Permission permission = PermissionBll.GetById(id);
            return View(permission);
        }
        public ActionResult Role(int id)
        {
            Permission permission = PermissionBll.GetById(id);
            return View(permission);
        }
        public ActionResult Function(int id)
        {
            Permission permission = PermissionBll.GetById(id);
            return View(permission);
        }

        public ActionResult NoHasUser(int id)
        {
            IList<UserInfo> list = new List<UserInfo>();
            PermissionBll.GetById(id).UserPermission.ToList().ForEach(p => list.Add(p.UserInfo));
            IEnumerable<UserInfo> userInfos = UserInfoBll.LoadEntities(r => true).ToList().Except(list);
            return ResultData(Mapper.Map<IList<UserInfoOutputDto>>(userInfos.ToList()));
        }

        public ActionResult UserList(int id)
        {
            IList<UserInfo> list = new List<UserInfo>();
            PermissionBll.GetById(id).UserPermission.ToList().ForEach(p => list.Add(p.UserInfo));
            return ResultData(Mapper.Map<IList<UserInfoOutputDto>>(list));
        }

        public ActionResult UpdatePermission(int id, string uids)
        {
            string[] strs = uids.Split(',');
            IQueryable<UserInfo> userInfos = UserInfoBll.LoadEntities(r => strs.Contains(r.Id.ToString()));
            Permission permission = PermissionBll.GetById(id);
            bool b = UserPermissionBll.DeleteEntity(u => u.PermissionId.Equals(permission.Id)) > 0;
            IList<UserPermission> list = new List<UserPermission>();
            userInfos.ToList().ForEach(p => list.Add(new UserPermission() { PermissionId = permission.Id, UserInfoId = p.Id, HasPermission = true }));
            UserPermissionBll.AddEntities(list);
            return ResultData(null, b, b ? "用户分配成功！" : "用户分配失败！");
        }

        public ActionResult NoHasRole(int id)
        {
            IEnumerable<Role> roles = RoleBll.LoadEntities(r => true).ToList().Except(PermissionBll.GetById(id).Role.ToList());
            return ResultData(Mapper.Map<IList<RoleOutputDto>>(roles.ToList()));
        }

        public ActionResult RoleList(int id)
        {
            return ResultData(Mapper.Map<IList<RoleOutputDto>>(PermissionBll.GetById(id).Role.ToList()));
        }

        public ActionResult UpdatePermissionRole(int id, string rids)
        {
            string[] strs = rids.Split(',');
            IQueryable<Role> roles = RoleBll.LoadEntities(r => strs.Contains(r.Id.ToString()));
            Permission permission = PermissionBll.GetById(id);
            permission.Role.Clear();
            roles.ToList().ForEach(r => permission.Role.Add(r));
            PermissionBll.UpdateEntity(permission);
            bool b = PermissionBll.SaveChanges() > 0;
            return ResultData(null, b, b ? "权限角色分配成功！" : "权限角色分配失败！");
        }

        public ActionResult GetFunction(int id, int type, int start = 1, int length = 10)
        {
            var search = Request["search[value]"];
            bool b = search.IsNullOrEmpty();
            Permission permission = PermissionBll.GetById(id);
            List<Function> list = permission.Function.Where(c => c.FunctionType == (type == 1 ? FunctionType.Menu : FunctionType.Operating) && (b || c.Controller.Contains(search) || c.Action.Contains(search) || c.Name.Contains(search))).OrderBy(c => c.Id).Skip(start - 1).Take(length).ToList();
            DataTableViewModel model = new DataTableViewModel() { data = Mapper.Map<IList<FunctionOutputDto>>(list), recordsFiltered = list.Count, recordsTotal = list.Count };
            return Content(model.ToJsonString());
        }
    }
}