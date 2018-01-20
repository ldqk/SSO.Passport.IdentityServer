using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Common;
using Masuit.Tools.Security;
using Models.Dto;
using Models.Entity;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class AppController : BaseController
    {
        #region 增删查改

        /// <summary>
        /// 添加客户端子系统
        /// </summary>
        /// <param name="name">子系统名字</param>
        /// <returns></returns>
        public ActionResult Add(string name)
        {
            if (ClientAppBll.Any(a => a.AppName.Equals(name)))
            {
                return ResultData(null, false, $"{name} 应用已经存在！");
            }

            string appid = Guid.NewGuid().ToString().MDString();
            ClientApp app = new ClientApp() { AppName = name, AppId = appid, AppSecret = appid.MDString(ConfigurationManager.AppSettings["BaiduAK"]) };
            app = ClientAppBll.AddEntitySaved(app);
            if (app != null)
            {
                return ResultData(app, true, "应用程序添加成功！");
            }

            return ResultData(null, false, "应用程序添加失败！");
        }

        /// <summary>
        /// 修改子系统信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">新名字</param>
        /// <returns></returns>
        public ActionResult Update(int id, string name)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app != null)
            {
                if (ClientAppBll.Any(a => a.AppName.Equals(name) && !a.AppName.Equals(app.AppName)))
                {
                    return ResultData(null, false, $"{name} 应用已经存在！");
                }
                app.AppName = name;
                bool b = ClientAppBll.UpdateEntitySaved(app);
                return ResultData(null, b, b ? "修改成功" : "修改失败！");
            }

            return ResultData(null, false, "未找到该应用程序！");
        }

        /// <summary>
        /// 删除应用程序
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "应用不存在！");
            }

            if (app.Preset)
            {
                return ResultData(null, false, "预置应用不能被删除！");
            }
            bool b = ClientAppBll.DeleteEntitySaved(a => a.Id.Equals(id) && !a.Preset) > 0;
            return ResultData(null, b, b ? "应用删除成功！" : "应用删除失败！");
        }

        /// <summary>
        /// 获取应用程序分页数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw">搜索关键词</param>
        /// <returns></returns>
        public ActionResult PageData(int page = 1, int size = 10, string kw = "")
        {
            var where = string.IsNullOrEmpty(kw) ? (Expression<Func<ClientApp, bool>>)(a => true) : (a => a.AppName.Contains(kw) || a.Description.Contains(kw));
            List<ClientAppOutputDto> list = ClientAppBll.LoadPageEntitiesNoTracking<int, ClientAppOutputDto>(page, size, out int total, where, a => a.Id, false).ToList();
            return PageResult(list, size, total);
        }

        /// <summary>
        /// 获取应用详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Get(int id)
        {
            ClientAppOutputDto app = ClientAppBll.GetById(id).Mapper<ClientAppOutputDto>();
            return ResultData(app);
        }

        /// <summary>
        /// 获取所有的子系统
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAll()
        {
            List<ClientAppOutputDto> apps = ClientAppBll.GetAllNoTracking<ClientAppOutputDto>().ToList();
            return ResultData(apps);
        }

        /// <summary>
        /// 切换应用的可用状态
        /// </summary>
        /// <param name="id">应用id</param>
        /// <param name="state">可用状态</param>
        /// <returns></returns>
        public ActionResult ToggleState(int id, bool state)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "应用不存在！");
            }

            if (app.Preset)
            {
                return ResultData(null, false, "预置应用的状态不能修改！");
            }
            app.Available = state;
            bool b = ClientAppBll.UpdateEntitySaved(app);
            return ResultData(null, b, b ? "状态切换成功！" : "状态切换失败！");
        }
        #endregion

        #region 多对多

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUsers(int id, string uids)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "未找到应用！");
            }

            List<UserInfo> users = UserInfoBll.LoadEntities(u => uids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { app.UserInfo.Add(u); });
            ClientAppBll.UpdateEntity(app);
            ClientAppBll.BulkSaveChanges();
            return ResultData(null, true, "添加用户成功！");
        }

        /// <summary>
        /// 从应用移除用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uids"></param>
        /// <returns></returns>
        public ActionResult RemoveUsers(int id, string uids)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "未找到应用！");
            }

            List<UserInfo> users = UserInfoBll.LoadEntities(u => uids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { app.UserInfo.Remove(u); });
            ClientAppBll.UpdateEntity(app);
            ClientAppBll.BulkSaveChanges();
            return ResultData(null, true, "移除用户成功！");
        }

        /// <summary>
        /// 添加用户组
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUserGroups(int id, string gids)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "未找到应用！");
            }

            List<UserGroup> groups = UserGroupBll.LoadEntities(u => gids.Contains(u.Id.ToString())).ToList();
            groups.ForEach(u => { app.UserGroup.Add(u); });
            ClientAppBll.UpdateEntity(app);
            ClientAppBll.BulkSaveChanges();
            return ResultData(null, true, "添加用户组成功！");
        }

        /// <summary>
        /// 从应用移除用户组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="gids"></param>
        /// <returns></returns>
        public ActionResult RemoveUserGroups(int id, string gids)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "未找到应用！");
            }

            List<UserGroup> groups = UserGroupBll.LoadEntities(u => gids.Contains(u.Id.ToString())).ToList();
            groups.ForEach(u => { app.UserGroup.Remove(u); });
            ClientAppBll.UpdateEntity(app);
            ClientAppBll.BulkSaveChanges();
            return ResultData(null, true, "移除用户组成功！");
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <returns></returns>
        public ActionResult AddRoles(int id, string rids)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "未找到应用！");
            }

            List<Role> roles = RoleBll.LoadEntities(u => rids.Contains(u.Id.ToString())).ToList();
            roles.ForEach(u => { app.Roles.Add(u); });
            ClientAppBll.UpdateEntity(app);
            ClientAppBll.BulkSaveChanges();
            return ResultData(null, true, "添加角色成功！");
        }

        /// <summary>
        /// 从应用移除角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rids"></param>
        /// <returns></returns>
        public ActionResult RemoveRoles(int id, string rids)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "未找到应用！");
            }

            List<Role> roles = RoleBll.LoadEntities(u => rids.Contains(u.Id.ToString())).ToList();
            roles.ForEach(u => { app.Roles.Remove(u); });
            ClientAppBll.UpdateEntity(app);
            ClientAppBll.BulkSaveChanges();
            return ResultData(null, true, "移除角色成功！");
        }

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPermissions(int id, string pids)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "未找到应用！");
            }

            List<Permission> permissions = PermissionBll.LoadEntities(u => pids.Contains(u.Id.ToString())).ToList();
            permissions.ForEach(u => { app.Permissions.Add(u); });
            ClientAppBll.UpdateEntity(app);
            ClientAppBll.BulkSaveChanges();
            return ResultData(null, true, "添加权限成功！");
        }

        /// <summary>
        /// 从应用移除权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pids"></param>
        /// <returns></returns>
        public ActionResult RemovePermissions(int id, string pids)
        {
            ClientApp app = ClientAppBll.GetById(id);
            if (app is null)
            {
                return ResultData(null, false, "未找到应用！");
            }

            List<Permission> permissions = PermissionBll.LoadEntities(u => pids.Contains(u.Id.ToString())).ToList();
            permissions.ForEach(u => { app.Permissions.Remove(u); });
            ClientAppBll.UpdateEntity(app);
            ClientAppBll.BulkSaveChanges();
            return ResultData(null, true, "移除权限成功！");
        }

        #endregion
    }
}