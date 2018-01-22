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
    public class PermissionController : BaseController
    {
        public IUserPermissionBll UserPermissionBll { get; set; }

        public PermissionController(IUserPermissionBll userPermissionBll)
        {
            UserPermissionBll = userPermissionBll;
        }

        #region 增删查改

        /// <summary>
        /// 添加或修改权限
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="PermissionName">新名字</param>
        /// <param name="ParentId">父级组</param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ActionResult Save(int Id = 0, string PermissionName = "", int? ParentId = null, string appid = "")
        {
            ParentId = ParentId == 0 ? null : ParentId;
            Permission permission = PermissionBll.GetById(Id);
            if (permission != null)
            {
                //修改
                if (PermissionBll.Any(a => a.PermissionName.Equals(PermissionName) && !a.PermissionName.Equals(permission.PermissionName)))
                {
                    return ResultData(null, false, $"{PermissionName} 权限已经存在！");
                }
                permission.PermissionName = PermissionName;
                permission.ParentId = ParentId;
                bool b = PermissionBll.UpdateEntitySaved(permission);
                return ResultData(null, b, b ? "修改成功" : "修改失败！");
            }
            //添加
            if (PermissionBll.Any(a => a.PermissionName.Equals(PermissionName)))
            {
                return ResultData(null, false, $"{PermissionName} 权限已经存在！");
            }

            permission = new Permission() { PermissionName = PermissionName, ParentId = ParentId };
            if (!string.IsNullOrEmpty(appid) && ClientAppBll.Any(a => a.AppId.Equals(appid)))
            {
                var app = ClientAppBll.GetFirstEntity(a => a.AppId.Equals(appid));
                app.Permissions.Add(permission);
                bool b = ClientAppBll.UpdateEntitySaved(app);
                return ResultData(null, b, b ? "权限添加成功!" : "权限添加失败!");
            }
            permission = PermissionBll.AddEntitySaved(permission);
            return permission != null ? ResultData(permission, true, "权限添加成功！") : ResultData(null, false, "权限添加失败！");
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            bool b = PermissionBll.DeleteByIdSaved(id);
            return ResultData(null, b, b ? "权限删除成功！" : "权限删除失败！");
        }

        /// <summary>
        /// 获取权限分页数据
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ActionResult PageData(string appid, int page = 1, int size = 10)
        {
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<Permission, bool>>)(g => true) : (g => g.ClientApp.Any(a => a.AppId.Equals(appid)));
            List<int> ids = PermissionBll.LoadPageEntitiesNoTracking<int, PermissionOutputDto>(page, size, out int total, where, a => a.Id, false).Select(g => g.Id).ToList();
            List<PermissionOutputDto> list = new List<PermissionOutputDto>();
            ids.ForEach(g =>
            {
                List<PermissionOutputDto> temp = PermissionBll.GetSelfAndChildrenByParentId(g).ToList();
                list.AddRange(temp);
            });
            return PageResult(list.DistinctBy(u => u.Id), size, total);
        }

        /// <summary>
        /// 获取应用所有的权限
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult GetAll(string appid, string kw)
        {
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<Permission, bool>>)(g => true) : (g => g.ClientApp.Any(a => a.AppId.Equals(appid)));
            List<int> ids = PermissionBll.LoadEntitiesNoTracking<PermissionOutputDto>(where).Select(g => g.Id).ToList();
            List<PermissionOutputDto> list = new List<PermissionOutputDto>();
            ids.ForEach(g =>
            {
                var raw = PermissionBll.GetSelfAndChildrenByParentId(g);
                var temp = string.IsNullOrEmpty(kw) ? raw.ToList() : raw.Where(u => u.PermissionName.Contains(kw)).ToList();
                list.AddRange(temp);
            });
            return ResultData(list.DistinctBy(u => u.Id));
        }

        /// <summary>
        /// 获取权限详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Get(int id)
        {
            PermissionOutputDto permission = PermissionBll.GetById(id).Mapper<PermissionOutputDto>();
            return ResultData(permission);
        }

        #endregion

        #region 权限配置

        /// <summary>
        /// 获取拥有该临时权限权限的用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyUsers(int id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<UserInfo, bool>> where;
            Expression<Func<UserInfo, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.UserPermission.All(c => c.PermissionId != id) && (u.Username.Contains(kw) || u.Email.Contains(kw) || u.PhoneNumber.Contains(kw));
                where2 = u => u.UserPermission.Any(c => c.PermissionId == id) && (u.Username.Contains(kw) || u.Email.Contains(kw) || u.PhoneNumber.Contains(kw));
            }
            else
            {
                where = u => u.UserPermission.All(c => c.PermissionId != id);
                where2 = u => u.UserPermission.Any(c => c.PermissionId == id);
            }
            List<UserInfoDto> not = UserInfoBll.LoadPageEntities<DateTime, UserInfoDto>(page, size, out int total1, where, u => u.LastLoginTime, false).ToList();//不属于该应用
            List<UserInfo> list = UserInfoBll.LoadPageEntities(page, size, out int total2, where2, u => u.Id, false).ToList();//属于该应用
            List<UserInfoDto> my = new List<UserInfoDto>();
            foreach (var p in list)
            {
                //判断有没有临时权限
                UserInfoDto per = p.Mapper<UserInfoDto>();
                per.HasPermission = p.UserPermission.Any(u => u.PermissionId.Equals(id) && u.UserInfoId == p.Id && u.HasPermission);
                my.Add(per);
            }
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 获取该权限的角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyRoles(int id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<Role, bool>> where;
            Expression<Func<Role, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.Permission.All(c => c.Id != id) && u.RoleName.Contains(kw);
                where2 = u => u.Permission.Any(c => c.Id == id) && u.RoleName.Contains(kw);
            }
            else
            {
                where = u => u.Permission.All(c => c.Id != id);
                where2 = u => u.Permission.Any(c => c.Id == id);
            }
            List<RoleOutputDto> not = RoleBll.LoadPageEntities<int, RoleOutputDto>(page, size, out int total1, where, u => u.Id, false).ToList();//不属于该应用
            List<RoleOutputDto> my = RoleBll.LoadPageEntities<int, RoleOutputDto>(page, size, out int total2, where2, u => u.Id, false).ToList();//属于该应用
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 获取该权限的应用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyApps(int id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<ClientApp, bool>> where;
            Expression<Func<ClientApp, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.Permissions.All(c => c.Id != id) && u.AppName.Contains(kw);
                where2 = u => u.Permissions.Any(c => c.Id == id) && u.AppName.Contains(kw);
            }
            else
            {
                where = u => u.Permissions.All(c => c.Id != id);
                where2 = u => u.Permissions.Any(c => c.Id == id);
            }
            List<ClientAppOutputDto> not = ClientAppBll.LoadPageEntities<int, ClientAppOutputDto>(page, size, out int total1, where, u => u.Id, false).ToList();//不属于该应用
            List<ClientAppOutputDto> my = ClientAppBll.LoadPageEntities<int, ClientAppOutputDto>(page, size, out int total2, where2, u => u.Id, false).ToList();//属于该应用
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 获取该权限的控制器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyControls(int id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<Control, bool>> where;
            Expression<Func<Control, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.Permission.All(c => c.Id != id) && u.Name.Contains(kw);
                where2 = u => u.Permission.Any(c => c.Id == id) && u.Name.Contains(kw);
            }
            else
            {
                where = u => u.Permission.All(c => c.Id != id);
                where2 = u => u.Permission.Any(c => c.Id == id);
            }
            List<ControlOutputDto> not = ControlBll.LoadPageEntities<int, ControlOutputDto>(page, size, out int total1, where, u => u.Id, false).ToList();//不属于该应用
            List<ControlOutputDto> my = ControlBll.LoadPageEntities<int, ControlOutputDto>(page, size, out int total2, where2, u => u.Id, false).ToList();//属于该应用
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 获取该权限的控制器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyMenus(int id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<Menu, bool>> where;
            Expression<Func<Menu, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.Permission.All(c => c.Id != id) && u.Name.Contains(kw) || (u.Url != null && u.Url.Contains(kw)) || (u.Route != null && u.Route.Contains(kw)) || (u.RouteName != null && u.RouteName.Contains(kw));
                where2 = u => u.Permission.Any(c => c.Id == id) && u.Name.Contains(kw) || (u.Url != null && u.Url.Contains(kw)) || (u.Route != null && u.Route.Contains(kw)) || (u.RouteName != null && u.RouteName.Contains(kw));
            }
            else
            {
                where = u => u.Permission.All(c => c.Id != id);
                where2 = u => u.Permission.Any(c => c.Id == id);
            }
            List<MenuOutputDto> not = MenuBll.LoadPageEntities<int, MenuOutputDto>(page, size, out int total1, where, u => u.Id, false).ToList();//不属于该应用
            List<MenuOutputDto> my = MenuBll.LoadPageEntities<int, MenuOutputDto>(page, size, out int total2, where2, u => u.Id, false).ToList();//属于该应用
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 为用户分配临时权限
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="uids">用户id集合</param>
        /// <returns></returns>
        public ActionResult AddUsers(int id, string uids)
        {
            Permission permission = PermissionBll.GetById(id);
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            List<UserInfo> users = UserInfoBll.LoadEntities(u => uids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { UserPermissionBll.AddEntity(new UserPermission() { Permission = permission, HasPermission = true, PermissionId = permission.Id, UserInfo = u, UserInfoId = u.Id }); });
            UserPermissionBll.BulkSaveChanges();
            return ResultData(null, true, "权限配置完成！");
        }

        /// <summary>
        /// 将用户移除临时权限
        /// </summary>
        /// <param name="id">角色id</param>
        /// <param name="uids">用户id集合</param>
        /// <returns></returns>
        public ActionResult RemoveUsers(int id, string uids)
        {
            bool b = UserPermissionBll.DeleteEntitySaved(p => p.PermissionId.Equals(id) && uids.Contains(p.UserInfoId.ToString())) > 0;
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }

        /// <summary>
        /// 切换用户的临时权限状态
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="uid">用户id</param>
        /// <param name="state">是否可用</param>
        /// <returns></returns>
        public ActionResult ToggleState(int id, Guid uid, bool state)
        {
            UserPermission permission = UserPermissionBll.GetFirstEntity(p => p.UserInfoId.Equals(uid) && p.PermissionId.Equals(id));
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            permission.HasPermission = state;
            bool b = UserPermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }

        /// <summary>
        /// 添加控制功能
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="cids">控制器id集合</param>
        /// <returns></returns>
        public ActionResult AddControls(int id, string cids)
        {
            Permission permission = PermissionBll.GetById(id);
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            List<Control> controls = ControlBll.LoadEntities(c => cids.Contains(c.Id.ToString())).ToList();
            controls.ForEach(c => permission.Controls.Add(c));
            bool b = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }

        /// <summary>
        /// 移除控制功能
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="cids">控制器id集合</param>
        /// <returns></returns>
        public ActionResult RemoveControls(int id, string cids)
        {
            Permission permission = PermissionBll.GetById(id);
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            List<Control> controls = ControlBll.LoadEntities(c => cids.Contains(c.Id.ToString())).ToList();
            controls.ForEach(c => permission.Controls.Remove(c));
            bool b = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="mids">菜单id集合</param>
        /// <returns></returns>
        public ActionResult AddMenus(int id, string mids)
        {
            Permission permission = PermissionBll.GetById(id);
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            List<Menu> menus = MenuBll.LoadEntities(m => mids.Contains(m.Id.ToString())).ToList();
            menus.ForEach(m => permission.Menu.Add(m));
            bool b = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }

        /// <summary>
        /// 移除菜单
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="mids">菜单id集合</param>
        /// <returns></returns>
        public ActionResult RemoveMenus(int id, string mids)
        {
            Permission permission = PermissionBll.GetById(id);
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            List<Menu> menus = MenuBll.LoadEntities(m => mids.Contains(m.Id.ToString())).ToList();
            menus.ForEach(m => permission.Menu.Remove(m));
            bool b = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }


        /// <summary>
        /// 添加客户端应用
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="aids">应用id集合</param>
        /// <returns></returns>
        public ActionResult AddApps(int id, string aids)
        {
            Permission permission = PermissionBll.GetById(id);
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            List<ClientApp> apps = ClientAppBll.LoadEntities(a => aids.Contains(a.Id.ToString())).ToList();
            apps.ForEach(a => permission.ClientApp.Add(a));
            bool b = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }

        /// <summary>
        /// 移除客户端子系统应用
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="aids">应用id集合</param>
        /// <returns></returns>
        public ActionResult RemoveApps(int id, string aids)
        {
            Permission permission = PermissionBll.GetById(id);
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            List<ClientApp> apps = ClientAppBll.LoadEntities(a => aids.Contains(a.Id.ToString())).ToList();
            apps.ForEach(a => permission.ClientApp.Remove(a));
            bool b = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="rids">角色id集合</param>
        /// <returns></returns>
        public ActionResult AddRoles(int id, string rids)
        {
            Permission permission = PermissionBll.GetById(id);
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            List<Role> roles = RoleBll.LoadEntities(r => rids.Contains(r.Id.ToString())).ToList();
            roles.ForEach(r => permission.Role.Add(r));
            bool b = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="id">权限id</param>
        /// <param name="rids">角色id集合</param>
        /// <returns></returns>
        public ActionResult RemoveRoles(int id, string rids)
        {
            Permission permission = PermissionBll.GetById(id);
            if (permission is null)
            {
                return ResultData(null, false, "未找到相应的权限信息！");
            }

            List<Role> apps = RoleBll.LoadEntities(r => rids.Contains(r.Id.ToString())).ToList();
            apps.ForEach(r => permission.Role.Remove(r));
            bool b = PermissionBll.UpdateEntitySaved(permission);
            return ResultData(null, b, b ? "权限配置完成！" : "权限配置失败！");
        }

        /// <summary>
        /// 改变父级权限
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