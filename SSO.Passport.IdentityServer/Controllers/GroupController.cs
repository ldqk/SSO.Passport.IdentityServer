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
    public class GroupController : BaseController
    {
        public IUserGroupRoleBll UserGroupRoleBll { get; set; }

        public GroupController(IUserGroupRoleBll userGroupRoleBll)
        {
            UserGroupRoleBll = userGroupRoleBll;
        }

        #region 增删查改

        /// <summary>
        /// 添加或修改用户组
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="GroupName">新名字</param>
        /// <param name="ParentId">父级组</param>
        /// <param name="appid">appid</param>
        /// <returns></returns>
        public ActionResult Save(int Id = 0, string GroupName = "", int? ParentId = null, string appid = "")
        {
            ParentId = ParentId == 0 ? null : ParentId;
            UserGroup group = UserGroupBll.GetById(Id);
            if (group != null)
            {
                //修改
                if (UserGroupBll.Any(a => a.GroupName.Equals(GroupName) && !a.GroupName.Equals(group.GroupName)))
                {
                    return ResultData(null, false, $"{GroupName} 用户组已经存在！");
                }
                group.GroupName = GroupName;
                group.ParentId = ParentId;
                bool b = UserGroupBll.UpdateEntitySaved(@group);
                return ResultData(null, b, b ? "修改成功" : "修改失败！");
            }
            //添加
            if (UserGroupBll.Any(a => a.GroupName.Equals(GroupName)))
            {
                return ResultData(null, false, $"{GroupName} 用户组已经存在！");
            }
            group = new UserGroup { GroupName = GroupName, ParentId = ParentId };
            if (!string.IsNullOrEmpty(appid) && ClientAppBll.Any(a => a.AppId.Equals(appid)))
            {
                var app = ClientAppBll.GetFirstEntity(a => a.AppId.Equals(appid));
                app.UserGroup.Add(group);
                bool b = ClientAppBll.UpdateEntitySaved(app);
                return ResultData(null, b, b ? "用户组添加成功!" : "用户组添加失败!");
            }
            group = UserGroupBll.AddEntitySaved(group);
            return @group != null ? ResultData(@group, true, "用户组添加成功！") : ResultData(null, false, "用户组添加失败！");
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
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<UserGroup, bool>>)(g => true) : (g => g.ClientApp.Any(a => a.AppId.Equals(appid)));
            List<int> ids = UserGroupBll.LoadPageEntitiesNoTracking<int, UserGroupOutputDto>(page, size, out int total, where, a => a.Id, false).Select(g => g.Id).ToList();
            List<UserGroupOutputDto> list = new List<UserGroupOutputDto>();
            ids.ForEach(g =>
            {
                List<UserGroupOutputDto> temp = UserGroupBll.GetSelfAndChildrenByParentId(g).ToList();
                list.AddRange(temp);
            });
            return PageResult(list.DistinctBy(u => u.Id), size, total);
        }

        /// <summary>
        /// 获取应用所有的用户组
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="kw">查询关键词</param>
        /// <returns></returns>
        public ActionResult GetAll(string appid, string kw)
        {
            var where = string.IsNullOrEmpty(appid) ? (Expression<Func<UserGroup, bool>>)(g => true) : (g => g.ClientApp.Any(a => a.AppId.Equals(appid)));
            UserGroupBll.LoadEntitiesNoTracking<UserGroupOutputDto>(where);
            List<int> ids = UserGroupBll.LoadEntitiesNoTracking<UserGroupOutputDto>(where).Select(g => g.Id).ToList();
            List<UserGroupOutputDto> list = new List<UserGroupOutputDto>();
            ids.ForEach(g =>
            {
                var raw = UserGroupBll.GetSelfAndChildrenByParentId(g);
                var temp = string.IsNullOrEmpty(kw) ? raw.ToList() : raw.Where(u => u.GroupName.Contains(kw)).ToList();
                list.AddRange(temp);
            });
            return ResultData(list.DistinctBy(u => u.Id));
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
        /// 获取该用户组的用户
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
                where = u => u.UserGroup.All(c => c.Id != id) && (u.Username.Contains(kw) || u.Email.Contains(kw) || u.PhoneNumber.Contains(kw));
                where2 = u => u.UserGroup.Any(c => c.Id == id) && (u.Username.Contains(kw) || u.Email.Contains(kw) || u.PhoneNumber.Contains(kw));
            }
            else
            {
                where = u => u.UserGroup.All(c => c.Id != id);
                where2 = u => u.UserGroup.Any(c => c.Id == id);
            }
            List<UserInfoDto> not = UserInfoBll.LoadPageEntities<DateTime, UserInfoDto>(page, size, out int total1, where, u => u.LastLoginTime, false).ToList();//不属于该应用
            List<UserInfoDto> my = UserInfoBll.LoadPageEntities<DateTime, UserInfoDto>(page, size, out int total2, where2, u => u.LastLoginTime, false).ToList();//属于该应用
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }


        /// <summary>
        /// 获取用户组归属的所有应用
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
                where = u => u.UserGroup.All(c => c.Id != id) && (u.AppName.Contains(kw) || u.AppId.Contains(kw));
                where2 = u => u.UserGroup.Any(c => c.Id == id) && (u.AppName.Contains(kw) || u.AppId.Contains(kw));
            }
            else
            {
                where = u => u.UserGroup.All(c => c.Id != id);
                where2 = u => u.UserGroup.Any(c => c.Id == id);
            }

            List<ClientAppOutputDto> not = ClientAppBll.LoadPageEntities<string, ClientAppOutputDto>(page, size, out int total1, where, u => u.AppName, false).ToList(); //不属于该应用
            List<ClientAppOutputDto> my = ClientAppBll.LoadPageEntities<string, ClientAppOutputDto>(page, size, out int total2, where2, u => u.AppName, false).ToList(); //属于该应用
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 获取该用户组的角色
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
                where = u => u.UserGroupPermission.All(c => c.UserGroupId != id) && u.RoleName.Contains(kw);
                where2 = u => u.UserGroupPermission.Any(c => c.UserGroupId == id) && u.RoleName.Contains(kw);
            }
            else
            {
                where = u => u.UserGroupPermission.All(c => c.UserGroupId != id);
                where2 = u => u.UserGroupPermission.Any(c => c.UserGroupId == id);
            }
            List<RoleOutputDto> not = RoleBll.LoadPageEntities<int, RoleOutputDto>(page, size, out int total1, where, u => u.Id, false).ToList();//不属于该应用
            List<Role> list = RoleBll.LoadPageEntities(page, size, out int total2, where2, u => u.Id, false).ToList();//属于该应用
            List<RoleOutputDto> my = new List<RoleOutputDto>();
            foreach (var p in list)
            {
                //判断有没有临时权限
                RoleOutputDto per = p.Mapper<RoleOutputDto>();
                per.HasRole = p.UserGroupPermission.Any(u => u.UserGroupId.Equals(id) && u.RoleId == p.Id && u.HasRole);
                my.Add(per);
            }
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUsers(int id, string uids)
        {
            string[] ids = uids.Split(',');
            UserGroup @group = UserGroupBll.GetById(id);
            if (@group is null)
            {
                return ResultData(null, false, "未找到用户组！");
            }

            List<UserInfo> users = UserInfoBll.LoadEntities(u => ids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { @group.UserInfo.Add(u); });
            bool b = UserGroupBll.UpdateEntitySaved(@group);
            return ResultData(null, b, b ? "添加用户成功！" : "添加用户失败！");
        }

        /// <summary>
        /// 从应用移除用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uids"></param>
        /// <returns></returns>
        public ActionResult RemoveUsers(int id, string uids)
        {
            string[] ids = uids.Split(',');
            UserGroup @group = UserGroupBll.GetById(id);
            if (@group is null)
            {
                return ResultData(null, false, "未找到用户组！");
            }

            List<UserInfo> users = UserInfoBll.LoadEntities(u => ids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { @group.UserInfo.Remove(u); });
            bool b = UserGroupBll.UpdateEntitySaved(@group);
            return ResultData(null, b, b ? "移除用户成功！" : "移除用户失败！");
        }

        /// <summary>
        /// 添加客户端子系统
        /// </summary>
        /// <returns></returns>
        public ActionResult AddApps(int id, string aids)
        {
            string[] ids = aids.Split(',');
            UserGroup @group = UserGroupBll.GetById(id);
            if (@group is null)
            {
                return ResultData(null, false, "未找到用户组！");
            }

            List<ClientApp> users = ClientAppBll.LoadEntities(u => ids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { @group.ClientApp.Add(u); });
            bool b = UserGroupBll.UpdateEntitySaved(@group);
            return ResultData(null, b, b ? "添加客户端子系统成功！" : "添加客户端子系统失败！");
        }

        /// <summary>
        /// 从应用移除用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="aids"></param>
        /// <returns></returns>
        public ActionResult RemoveApps(int id, string aids)
        {
            string[] ids = aids.Split(',');
            UserGroup @group = UserGroupBll.GetById(id);
            if (@group is null)
            {
                return ResultData(null, false, "未找到用户组！");
            }

            List<ClientApp> users = ClientAppBll.LoadEntities(u => ids.Contains(u.Id.ToString())).ToList();
            users.ForEach(u => { @group.ClientApp.Remove(u); });
            bool b = UserGroupBll.UpdateEntitySaved(@group);
            return ResultData(null, b, b ? "移除客户端子系统成功！" : "移除客户端子系统失败！");
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="id">用户组id</param>
        /// <param name="rids">角色id</param>
        /// <returns></returns>
        public ActionResult AddRoles(int id, string rids)
        {
            string[] ids = rids.Split(',');
            var @group = UserGroupBll.GetById(id);
            if (group is null)
            {
                return ResultData(null, false, "用户组不存在");
            }

            List<Role> roles = RoleBll.LoadEntities(r => ids.Contains(r.Id.ToString())).ToList();
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
            string[] ids = rids.Split(',');
            UserGroupRoleBll.DeleteEntitySaved(r => ids.Contains(r.RoleId.ToString()) && r.UserGroupId.Equals(id));
            return ResultData(null, true, "角色配置完成！");
        }

        /// <summary>
        /// 切换角色是否可用
        /// </summary>
        /// <param name="id">用户组id</param>
        /// <param name="rid">角色id</param>
        /// <param name="state">是否可用</param>
        /// <returns></returns>
        public ActionResult ToggleState(int id, int rid, bool state)
        {
            UserGroupRole role = UserGroupRoleBll.GetFirstEntity(u => u.UserGroupId.Equals(id) && u.RoleId.Equals(rid));
            role.HasRole = !state;
            bool b = UserGroupRoleBll.UpdateEntitySaved(role);
            return ResultData(null, b, b ? "用户组状态切换成功！" : "用户组状态切换失败！");
        }
        #endregion
    }
}