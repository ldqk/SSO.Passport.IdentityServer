using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Common;
using IBLL;
using Microsoft.Ajax.Utilities;
using Models.Dto;
using Models.Entity;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class RoleController : BaseController
    {
        public IUserGroupRoleBll UserGroupRoleBll { get; set; }

        public RoleController(IUserGroupRoleBll userGroupRoleBll)
        {
            UserGroupRoleBll = userGroupRoleBll;
        }
        #region 增删查改

        /// <summary>
        /// 添加或修改角色
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="RoleName">新名字</param>
        /// <param name="ParentId">父级组</param>
        /// <param name="appid">appid</param>
        /// <returns></returns>
        public ActionResult Save(int Id = 0, string RoleName = "", int? ParentId = null, string appid = "")
        {
            ParentId = ParentId == 0 ? null : ParentId;
            Role role = RoleBll.GetById(Id);
            if (role != null)
            {
                //修改
                if (RoleBll.Any(a => a.RoleName.Equals(RoleName) && !a.RoleName.Equals(role.RoleName)))
                {
                    return ResultData(null, false, $"{RoleName} 角色已经存在！");
                }
                role.RoleName = RoleName;
                role.ParentId = ParentId;
                bool b = RoleBll.UpdateEntitySaved(role);
                return ResultData(null, b, b ? "修改成功" : "修改失败！");
            }
            //添加
            if (RoleBll.Any(a => a.RoleName.Equals(RoleName)))
            {
                return ResultData(null, false, $"{RoleName} 角色已经存在！");
            }
            role = new Role() { RoleName = RoleName, ParentId = ParentId };
            if (!string.IsNullOrEmpty(appid) && ClientAppBll.Any(a => a.AppId.Equals(appid)))
            {
                var app = ClientAppBll.GetFirstEntity(a => a.AppId.Equals(appid));
                app.Roles.Add(role);
                bool b = ClientAppBll.UpdateEntitySaved(app);
                return ResultData(null, b, b ? "角色添加成功!" : "角色添加失败!");
            }
            role = RoleBll.AddEntitySaved(role);
            return role != null ? ResultData(role, true, "角色添加成功！") : ResultData(null, false, "角色添加失败！");
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            bool b = RoleBll.DeleteByIdSaved(id);
            return ResultData(null, b, b ? "角色删除成功！" : "角色删除失败！");
        }

        /// <summary>
        /// 获取角色分页数据
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ActionResult PageData(string appid, int page = 1, int size = 10)
        {
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<Role, bool>>)(g => true) : (g => g.ClientApp.Any(a => a.AppId.Equals(appid)));
            List<int> ids = RoleBll.LoadPageEntitiesNoTracking<int, RoleOutputDto>(page, size, out int total, where, a => a.Id, false).Select(g => g.Id).ToList();
            List<RoleOutputDto> list = new List<RoleOutputDto>();
            ids.ForEach(g =>
            {
                List<RoleOutputDto> temp = RoleBll.GetSelfAndChildrenByParentId(g).ToList();
                list.AddRange(temp);
            });
            return PageResult(list.DistinctBy(u => u.Id), size, total);
        }

        /// <summary>
        /// 获取应用所有的角色
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult GetAll(string appid, string kw)
        {
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<Role, bool>>)(g => true) : (g => g.ClientApp.Any(a => a.AppId.Equals(appid)));
            List<int> ids = RoleBll.LoadEntitiesNoTracking<RoleOutputDto>(where).Select(g => g.Id).ToList();
            List<RoleOutputDto> list = new List<RoleOutputDto>();
            ids.ForEach(g =>
            {
                var raw = RoleBll.GetSelfAndChildrenByParentId(g);
                var temp = string.IsNullOrEmpty(kw) ? raw.ToList() : raw.Where(u => u.RoleName.Contains(kw)).ToList();
                list.AddRange(temp);
            });
            return ResultData(list.DistinctBy(u => u.Id));
        }

        /// <summary>
        /// 获取角色详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Get(int id)
        {
            RoleOutputDto role = RoleBll.GetById(id).Mapper<RoleOutputDto>();
            return ResultData(role);
        }

        #endregion
        #region 角色配置

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="uids">用户id集合</param>
        /// <returns></returns>
        public ActionResult AddUsers(int id, string uids)
        {
            Role role = RoleBll.GetById(id);
            if (role is null)
            {
                return ResultData(null, false, "未找到相应的角色信息！");
            }

            List<UserInfo> users = UserInfoBll.LoadEntities(u => uids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => role.UserInfo.Add(u));
            RoleBll.BulkSaveChanges();
            return ResultData(null, true, "角色配置完成！");
        }

        /// <summary>
        /// 将用户移除角色
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="uids">用户id集合</param>
        /// <returns></returns>
        public ActionResult RemoveUsers(int id, string uids)
        {
            Role role = RoleBll.GetById(id);
            if (role is null)
            {
                return ResultData(null, false, "未找到相应的角色信息！");
            }

            List<UserInfo> users = UserInfoBll.LoadEntities(u => uids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => role.UserInfo.Remove(u));
            RoleBll.BulkSaveChanges();
            return ResultData(null, true, "角色配置完成！");
        }

        /// <summary>
        /// 切换用户组的角色状态
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="gid">用户组id</param>
        /// <param name="state">是否可用</param>
        /// <returns></returns>
        public ActionResult ToggleState(int id, int gid, bool state)
        {
            UserGroupRole role = UserGroupRoleBll.GetFirstEntity(p => p.UserGroupId.Equals(gid) && p.RoleId.Equals(id));
            if (role is null)
            {
                return ResultData(null, false, "未找到相应的角色信息！");
            }

            role.HasRole = state;
            bool b = UserGroupRoleBll.UpdateEntitySaved(role);
            return ResultData(null, b, b ? "角色配置完成！" : "角色配置失败！");
        }

        /// <summary>
        /// 添加用户组
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="gids">用户组id集合</param>
        /// <returns></returns>
        public ActionResult AddGroups(int id, string gids)
        {
            Role role = RoleBll.GetById(id);
            if (role is null)
            {
                return ResultData(null, false, "未找到相应的角色信息！");
            }

            List<UserGroup> controls = UserGroupBll.LoadEntities(g => gids.Contains(g.Id.ToString())).ToList();
            controls.ForEach(g => UserGroupRoleBll.AddEntity(new UserGroupRole() { UserGroup = g, Role = role, HasRole = true, RoleId = role.Id, UserGroupId = g.Id }));
            UserGroupRoleBll.BulkSaveChanges();
            return ResultData(null, true, "角色配置完成！");
        }

        /// <summary>
        /// 移除用户组
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="gids">用户组id集合</param>
        /// <returns></returns>
        public ActionResult RemoveGroups(int id, string gids)
        {
            bool b = UserGroupRoleBll.DeleteEntitySaved(r => r.RoleId.Equals(id) && gids.Contains(r.UserGroupId.ToString())) > 0;
            return ResultData(null, b, b ? "角色配置完成！" : "角色配置失败！");
        }

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="pids">权限id集合</param>
        /// <returns></returns>
        public ActionResult AddPermissions(int id, string pids)
        {
            Role role = RoleBll.GetById(id);
            if (role is null)
            {
                return ResultData(null, false, "未找到相应的角色信息！");
            }

            List<Permission> permissions = PermissionBll.LoadEntities(p => pids.Contains(p.Id.ToString())).ToList();
            permissions.ForEach(p => role.Permission.Add(p));
            RoleBll.UpdateEntity(role);
            RoleBll.BulkSaveChanges();
            return ResultData(null, true, "角色配置完成！");
        }

        /// <summary>
        /// 移除权限
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="pids">权限id集合</param>
        /// <returns></returns>
        public ActionResult RemovePermissions(int id, string pids)
        {
            Role role = RoleBll.GetById(id);
            if (role is null)
            {
                return ResultData(null, false, "未找到相应的角色信息！");
            }

            List<Permission> permissions = PermissionBll.LoadEntities(p => pids.Contains(p.Id.ToString())).ToList();
            permissions.ForEach(p => role.Permission.Remove(p));
            RoleBll.UpdateEntity(role);
            RoleBll.BulkSaveChanges();
            return ResultData(null, true, "角色配置完成！");
        }


        /// <summary>
        /// 添加客户端应用
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="aids">应用id集合</param>
        /// <returns></returns>
        public ActionResult AddApps(int id, string aids)
        {
            Role role = RoleBll.GetById(id);
            if (role is null)
            {
                return ResultData(null, false, "未找到相应的角色信息！");
            }

            List<ClientApp> apps = ClientAppBll.LoadEntities(a => aids.Contains(a.Id.ToString())).ToList();
            apps.ForEach(a => role.ClientApp.Add(a));
            RoleBll.UpdateEntity(role);
            RoleBll.BulkSaveChanges();
            return ResultData(null, true, "角色配置完成！");
        }

        /// <summary>
        /// 移除客户端子系统应用
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="aids">应用id集合</param>
        /// <returns></returns>
        public ActionResult RemoveApps(int id, string aids)
        {
            Role role = RoleBll.GetById(id);
            if (role is null)
            {
                return ResultData(null, false, "未找到相应的角色信息！");
            }

            List<ClientApp> apps = ClientAppBll.LoadEntities(a => aids.Contains(a.Id.ToString())).ToList();
            apps.ForEach(a => role.ClientApp.Remove(a));
            RoleBll.UpdateEntity(role);
            RoleBll.BulkSaveChanges();
            return ResultData(null, true, "角色配置完成！");
        }

        /// <summary>
        /// 改变父级角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult ChangeParent(int id, int? pid)
        {
            Permission permission = PermissionBll.GetById(id);
            if (pid.HasValue)
            {
                Permission parent = PermissionBll.GetById(pid);
                if (parent is null)
                {
                    pid = null;
                }
            }

            permission.ParentId = pid;
            bool b = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "父级指派成功！" : "父级指派失败！");
        }
        #endregion
    }
}