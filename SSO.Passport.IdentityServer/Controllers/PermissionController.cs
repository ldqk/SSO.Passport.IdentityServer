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


        public ActionResult Get(int id)
        {
            PermissionOutputDto model = Mapper.Map<PermissionOutputDto>(PermissionBll.GetById(id));
            return ResultData(model);
        }

        public ActionResult GetAllList()
        {
            IQueryable<Permission> permissions = PermissionBll.LoadEntitiesNoTracking(r => true);
            IList<PermissionOutputDto> list = Mapper.Map<IList<PermissionOutputDto>>(permissions.ToList());
            return ResultData(list, permissions.Any());
        }

        public ActionResult GetPageData(int page = 1, int size = 10)
        {
            IQueryable<Permission> permissions = PermissionBll.LoadPageEntitiesNoTracking(page, size, out int totalCount, r => true, r => r.Id);
            PageDataViewModel model = new PageDataViewModel() { Data = Mapper.Map<IList<PermissionOutputDto>>(permissions.ToList()), PageIndex = page, PageSize = size, TotalPage = Math.Ceiling(totalCount.To<double>() / size.To<double>()).ToInt32(), TotalCount = totalCount };
            return ResultData(model, permissions.Any());
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

        public ActionResult Toggle(Guid id, int pid, bool allow)
        {
            List<UserPermission> list = UserPermissionBll.LoadEntities(p => p.UserInfoId.Equals(id) && p.PermissionId.Equals(pid)).ToList();
            list.ForEach(p => p.HasPermission = allow);
            bool b = UserPermissionBll.UpdateEntities(list);
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

    }
}