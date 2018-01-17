using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Common;
using IBLL;
using Models.Dto;
using Models.Entity;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class GroupController : BaseController
    {
        public IUserGroupRoleBll UserGroupRoleBll { get; set; }

        public GroupController(IUserGroupRoleBll userGroupRoleBll)
        {
            UserGroupRoleBll = userGroupRoleBll;
        }

        #region 增删查改

        /// <summary>
        /// 添加用户组
        /// </summary>
        /// <param name="name">用户组名</param>
        /// <param name="pid">父级组</param>
        /// <returns></returns>
        public ActionResult Add(string name, int? pid)
        {
            if (UserGroupBll.Any(a => a.GroupName.Equals(name)))
            {
                return ResultData(null, false, $"{name} 用户组已经存在！");
            }

            UserGroup group = new UserGroup() { GroupName = name, ParentId = pid };
            group = UserGroupBll.AddEntitySaved(group);
            if (group != null)
            {
                return ResultData(group, true, "用户组添加成功！");
            }

            return ResultData(null, false, "用户组添加失败！");
        }

        /// <summary>
        /// 修改用户组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">新名字</param>
        /// <param name="pid">父级组</param>
        /// <returns></returns>
        public ActionResult Update(int id, string name, int? pid)
        {
            UserGroup @group = UserGroupBll.GetById(id);
            if (@group != null)
            {
                @group.GroupName = name;
                @group.ParentId = id;
                bool b = UserGroupBll.UpdateEntitySaved(@group);
                return ResultData(null, b, b ? "修改成功" : "修改失败！");
            }

            return ResultData(null, false, "未找到用户组！");
        }

        /// <summary>
        /// 删除用户组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            bool b = UserGroupBll.DeleteByIdSaved(id);
            return ResultData(null, b, b ? "用户组删除成功！" : "用户组删除失败！");
        }

        /// <summary>
        /// 获取用户组分页数据
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ActionResult PageData(string appid, int page = 1, int size = 10)
        {
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<UserGroup, bool>>)(g => true) : (g => g.ClientApp.Any(a => a.AppName.Equals(appid)));
            List<int> ids = UserGroupBll.LoadPageEntitiesNoTracking<int, UserGroupOutputDto>(page, size, out int total, where, a => a.Id, false).Select(g => g.Id).ToList();
            List<UserGroupOutputDto> list = new List<UserGroupOutputDto>();
            ids.ForEach(g =>
            {
                List<UserGroupOutputDto> temp = UserGroupBll.GetSelfAndChildrenByParentId(g).ToList();
                list.AddRange(temp);
            });
            return PageResult(list, size, total);
        }

        /// <summary>
        /// 获取应用所有的用户组
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        public ActionResult GetAll(string appid)
        {
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<UserGroup, bool>>)(g => true) : (g => g.ClientApp.Any(a => a.AppName.Equals(appid)));
            List<int> ids = UserGroupBll.LoadEntitiesNoTracking<UserGroupOutputDto>(where).Select(g => g.Id).ToList();
            List<UserGroupOutputDto> list = new List<UserGroupOutputDto>();
            ids.ForEach(g =>
            {
                List<UserGroupOutputDto> temp = UserGroupBll.GetSelfAndChildrenByParentId(g).ToList();
                list.AddRange(temp);
            });
            return ResultData(list);
        }

        /// <summary>
        /// 获取用户组详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Get(int id)
        {
            UserGroupOutputDto group = UserGroupBll.GetById(id).Mapper<UserGroupOutputDto>();
            return ResultData(group);
        }

        #endregion

        #region 权限配置

        /// <summary>
        /// 改变父级组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult ChangeParent(int id, int? pid)
        {
            UserGroup @group = UserGroupBll.GetById(id);
            if (pid.HasValue)
            {
                UserGroup parent = UserGroupBll.GetById(pid);
                if (parent is null)
                {
                    pid = null;
                }
            }

            group.ParentId = pid;
            bool b = UserGroupBll.UpdateEntitySaved(@group);
            return ResultData(null, b, b ? "父级指派成功！" : "父级指派失败！");
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUsers(int id, string uids)
        {
            UserGroup @group = UserGroupBll.GetById(id);
            if (@group is null)
            {
                return ResultData(null, false, "未找到用户组！");
            }

            List<UserInfo> users = UserInfoBll.LoadEntities(u => uids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { @group.UserInfo.Add(u); });
            UserGroupBll.UpdateEntity(@group);
            UserGroupBll.BulkSaveChanges();
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
            UserGroup @group = UserGroupBll.GetById(id);
            if (@group is null)
            {
                return ResultData(null, false, "未找到用户组！");
            }

            List<UserInfo> users = UserInfoBll.LoadEntities(u => uids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { @group.UserInfo.Remove(u); });
            UserGroupBll.UpdateEntity(@group);
            UserGroupBll.BulkSaveChanges();
            return ResultData(null, true, "移除用户成功！");
        }

        /// <summary>
        /// 添加客户端子系统
        /// </summary>
        /// <returns></returns>
        public ActionResult AddApps(int id, string aids)
        {
            UserGroup @group = UserGroupBll.GetById(id);
            if (@group is null)
            {
                return ResultData(null, false, "未找到用户组！");
            }

            List<ClientApp> users = ClientAppBll.LoadEntities(u => aids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { @group.ClientApp.Add(u); });
            UserGroupBll.UpdateEntity(@group);
            UserGroupBll.BulkSaveChanges();
            return ResultData(null, true, "添加客户端子系统成功！");
        }

        /// <summary>
        /// 从应用移除用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="aids"></param>
        /// <returns></returns>
        public ActionResult RemoveApps(int id, string aids)
        {
            UserGroup @group = UserGroupBll.GetById(id);
            if (@group is null)
            {
                return ResultData(null, false, "未找到用户组！");
            }

            List<ClientApp> users = ClientAppBll.LoadEntities(u => aids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { @group.ClientApp.Remove(u); });
            UserGroupBll.UpdateEntity(@group);
            UserGroupBll.BulkSaveChanges();
            return ResultData(null, true, "移除客户端子系统成功！");
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="id">用户组id</param>
        /// <param name="rids">角色id</param>
        /// <returns></returns>
        public ActionResult AddRoles(int id, string rids)
        {
            var @group = UserGroupBll.GetById(id);
            if (group is null)
            {
                return ResultData(null, false, "用户组不存在");
            }

            List<Role> roles = RoleBll.LoadEntities(r => rids.Contains(r.Id.ToString())).ToList();
            roles.ForEach(r =>
            {
                UserGroupRole groupRole = new UserGroupRole() { UserGroupId = @group.Id, HasRole = true, RoleId = r.Id, Role = r, UserGroup = @group };
                UserGroupRoleBll.AddEntity(groupRole);
            });
            UserGroupRoleBll.BulkSaveChanges();
            return ResultData(null, true, "角色配置完成！");
        }

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="id">用户组id</param>
        /// <param name="rids">角色id集合</param>
        /// <returns></returns>
        public ActionResult RemoveRoles(int id, string rids)
        {
            var @group = UserGroupBll.GetById(id);
            if (group is null)
            {
                return ResultData(null, false, "用户组不存在");
            }

            bool b = UserGroupRoleBll.DeleteEntitySaved(r => rids.Contains(r.Id.ToString()) && r.UserGroupId.Equals(id)) > 0;
            return ResultData(null, b, b ? "角色配置完成！" : "角色配置失败！");
        }

        #endregion
    }
}