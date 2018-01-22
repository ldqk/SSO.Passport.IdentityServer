using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using AutoMapper;
using Common;
using IBLL;
using Models.Dto;
using Models.Entity;

namespace SSO.Passport.IdentityServer.Controllers
{
    public class UserController : BaseController
    {
        public IUserPermissionBll UserPermissionBll { get; set; }

        public UserController(IUserPermissionBll userPermissionBll)
        {
            UserPermissionBll = userPermissionBll;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="old"></param>
        /// <param name="pwd"></param>
        /// <param name="confirm"></param>
        /// <returns></returns>
        public ActionResult ChangePassword(string old, string pwd, string confirm)
        {
            if (pwd.Length <= 6)
            {
                return ResultData(null, false, "密码过短，至少需要6个字符！");
            }

            if (!pwd.Equals(confirm))
            {
                return ResultData(null, false, "两次输入的密码不一致！");
            }

            var regex = new Regex(@"(?=.*[0-9])                     #必须包含数字
                                            (?=.*[a-zA-Z])                  #必须包含小写或大写字母
                                            (?=([\x21-\x7e]+)[^a-zA-Z0-9])  #必须包含特殊符号
                                            .{6,30}                         #至少6个字符，最多30个字符
                                            ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            if (regex.Match(pwd).Success)
            {
                bool b = UserInfoBll.ChangePassword(CurrentUser.Id, old, pwd);
                return ResultData(null, b, b ? $"密码修改成功，新密码为：{pwd}！" : "密码修改失败，可能是原密码不正确！");
            }

            return ResultData(null, false, "密码强度值不够，密码必须包含数字，必须包含小写或大写字母，必须包含至少一个特殊符号，至少6个字符，最多30个字符！");
        }

        /// <summary>
        /// 修改用户名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ActionResult ChangeUsername(string username)
        {
            UserInfo userInfo = UserInfoBll.GetById(CurrentUser.Id);
            if (!username.Equals(userInfo.Username) && UserInfoBll.UsernameExist(username))
            {
                return ResultData(null, false, $"用户名{username}已经存在，请尝试更换其他用户名！");
            }

            userInfo.Username = username;
            bool b = UserInfoBll.UpdateEntitySaved(userInfo);
            return ResultData(Mapper.Map<UserInfoDto>(userInfo), b, b ? $"用户名修改成功，新用户名为{username}。" : "用户名修改失败！");
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            bool b = UserInfoBll.DeleteByIdSaved(id);
            return ResultData(null, b, b ? "用户删除成功！" : "用户删除失败！");
        }

        /// <summary>
        /// 获取用户列表分页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult Page(int page = 1, int size = 10, string kw = "")
        {
            var @where = string.IsNullOrEmpty(kw) ? (Expression<Func<UserInfo, bool>>)(u => true) : (u => u.Username.Contains(kw) || u.Email.Contains(kw) || u.PhoneNumber.Contains(kw));
            List<UserInfoDto> list = UserInfoBll.LoadPageEntitiesNoTracking<DateTime, UserInfoDto>(page, size, out int total, where, u => u.LastLoginTime, false).ToList();
            return PageResult(list, size, total);
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Get(Guid id)
        {
            UserInfoDto user = UserInfoBll.GetById(id).Mapper<UserInfoDto>();
            return ResultData(user);
        }

        /// <summary>
        /// 修改用户头像
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult ChangeAvatar(Guid id, string path)
        {
            UserInfo userInfo = UserInfoBll.GetById(id);
            userInfo.Avatar = path;
            bool b = UserInfoBll.UpdateEntitySaved(userInfo);
            return ResultData(Mapper.Map<UserInfoDto>(userInfo), b, b ? $"头像修改成功。" : "头像修改失败！");
        }

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="state">用户状态</param>
        /// <returns></returns>
        public ActionResult LockUser(Guid id, bool state)
        {
            UserInfo user = UserInfoBll.GetById(id);
            if (user != null)
            {
                if (user.IsPreset)
                {
                    return ResultData(null, false, "内置管理员不可禁用！");
                }

                user.Locked = !state;
                bool b = UserInfoBll.UpdateEntitySaved(user);
                return ResultData(null, b, b ? "用户状态切换成功！" : "用户状态切换失败！");
            }

            return ResultData(null, false, "未找到用户！");
        }

        #region 权限配置

        /// <summary>
        /// 获取用户归属的所有应用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyApps(Guid id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<ClientApp, bool>> where;
            Expression<Func<ClientApp, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.UserInfo.All(c => c.Id != id) && (u.AppName.Contains(kw) || u.AppId.Contains(kw));
                where2 = u => u.UserInfo.Any(c => c.Id == id) && (u.AppName.Contains(kw) || u.AppId.Contains(kw));
            }
            else
            {
                where = u => u.UserInfo.All(c => c.Id != id);
                where2 = u => u.UserInfo.Any(c => c.Id == id);
            }

            List<ClientAppOutputDto> not = ClientAppBll.LoadPageEntities<string, ClientAppOutputDto>(page, size, out int total1, where, u => u.AppName, false).ToList(); //不属于该应用
            List<ClientAppOutputDto> my = ClientAppBll.LoadPageEntities<string, ClientAppOutputDto>(page, size, out int total2, where2, u => u.AppName, false).ToList(); //属于该应用
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 获取该应用的用户组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyGroups(Guid id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<UserGroup, bool>> where;
            Expression<Func<UserGroup, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.UserInfo.All(c => c.Id != id) && u.GroupName.Contains(kw);
                where2 = u => u.UserInfo.Any(c => c.Id == id) && u.GroupName.Contains(kw);
            }
            else
            {
                where = u => u.UserInfo.All(c => c.Id != id);
                where2 = u => u.UserInfo.Any(c => c.Id == id);
            }
            List<UserGroupOutputDto> not = UserGroupBll.LoadPageEntities<int, UserGroupOutputDto>(page, size, out int total1, where, u => u.Id, false).ToList();//不属于该应用
            List<UserGroupOutputDto> my = UserGroupBll.LoadPageEntities<int, UserGroupOutputDto>(page, size, out int total2, where2, u => u.Id, false).ToList();//属于该应用
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 获取该应用的角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyRoles(Guid id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<Role, bool>> where;
            Expression<Func<Role, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.UserInfo.All(c => c.Id != id) && u.RoleName.Contains(kw);
                where2 = u => u.UserInfo.Any(c => c.Id == id) && u.RoleName.Contains(kw);
            }
            else
            {
                where = u => u.UserInfo.All(c => c.Id != id);
                where2 = u => u.UserInfo.Any(c => c.Id == id);
            }
            List<RoleOutputDto> not = RoleBll.LoadPageEntities<int, RoleOutputDto>(page, size, out int total1, where, u => u.Id, false).ToList();//不属于该应用
            List<RoleOutputDto> my = RoleBll.LoadPageEntities<int, RoleOutputDto>(page, size, out int total2, where2, u => u.Id, false).ToList();//属于该应用
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 获取该应用的权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        public ActionResult MyPermissions(Guid id, int page = 1, int size = 10, string kw = "")
        {
            Expression<Func<Permission, bool>> where;
            Expression<Func<Permission, bool>> where2;
            if (!string.IsNullOrEmpty(kw))
            {
                where = u => u.UserPermission.All(c => c.UserInfoId != id) && u.PermissionName.Contains(kw);
                where2 = u => u.UserPermission.Any(c => c.UserInfoId == id) && u.PermissionName.Contains(kw);
            }
            else
            {
                where = u => u.UserPermission.All(c => c.UserInfoId != id);
                where2 = u => u.UserPermission.Any(c => c.UserInfoId == id);
            }
            List<PermissionOutputDto> not = PermissionBll.LoadPageEntities<int, PermissionOutputDto>(page, size, out int total1, where, u => u.Id, false).ToList();//不属于该应用
            List<Permission> list = PermissionBll.LoadPageEntities(page, size, out int total2, where2, u => u.Id, false).ToList();//属于该应用
            List<PermissionOutputDto> my = new List<PermissionOutputDto>();
            foreach (var p in list)
            {
                //判断有没有临时权限
                PermissionOutputDto per = p.Mapper<PermissionOutputDto>();
                per.HasPermission = p.UserPermission.Any(u => u.UserInfoId.Equals(id) && u.PermissionId == p.Id && u.HasPermission);
                my.Add(per);
            }
            return PageResult(new { my, not }, size, total1 >= total2 ? total1 : total2);
        }

        /// <summary>
        /// 分配给客户端子系统
        /// </summary>
        /// <param name="id"></param>
        /// <param name="aids"></param>
        /// <returns></returns>
        public ActionResult AddApps(Guid id, string aids)
        {
            string[] ids = aids.Split(',');
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<ClientApp> apps = ClientAppBll.LoadEntities(a => ids.Contains(a.Id.ToString())).ToList();
            apps.ForEach(a => user.ClientApp.Add(a));
            bool b = UserInfoBll.UpdateEntitySaved(user);
            return ResultData(null, b, b ? "分配给客户端子系统完成！" : "分配给客户端子系统失败！");
        }

        /// <summary>
        /// 添加到用户组
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="gids">用户组id</param>
        /// <returns></returns>
        public ActionResult AddGroups(Guid id, string gids)
        {
            string[] ids = gids.Split(',');
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<UserGroup> groups = UserGroupBll.LoadEntities(g => ids.Contains(g.Id.ToString())).ToList();
            groups.ForEach(g => user.UserGroup.Add(g));
            bool b = UserInfoBll.UpdateEntitySaved(user);
            return ResultData(null, b, b ? "分配到用户组完成！" : "分配到用户组失败！");
        }

        /// <summary>
        /// 移除用户组
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="gids">用户组id</param>
        /// <returns></returns>
        public ActionResult RemoveGroups(Guid id, string gids)
        {
            string[] ids = gids.Split(',');
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<UserGroup> groups = UserGroupBll.LoadEntities(g => ids.Contains(g.Id.ToString())).ToList();
            groups.ForEach(g => user.UserGroup.Remove(g));
            bool b = UserInfoBll.UpdateEntitySaved(user);
            return ResultData(null, b, b ? "移除用户组完成！" : "移除用户组失败！");
        }

        /// <summary>
        /// 添加到角色
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="rids">角色id</param>
        /// <returns></returns>
        public ActionResult AddRoles(Guid id, string rids)
        {
            string[] ids = rids.Split(',');
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<Role> roles = RoleBll.LoadEntities(r => ids.Contains(r.Id.ToString())).ToList();
            roles.ForEach(r => user.Role.Add(r));
            bool b = UserInfoBll.UpdateEntitySaved(user);
            return ResultData(null, b, b ? "分配角色完成！" : "分配角色失败！");
        }

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="rids">角色id</param>
        /// <returns></returns>
        public ActionResult RemoveRoles(Guid id, string rids)
        {
            string[] ids = rids.Split(',');
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<Role> groups = RoleBll.LoadEntities(r => ids.Contains(r.Id.ToString())).ToList();
            groups.ForEach(r => user.Role.Remove(r));
            bool b = UserInfoBll.UpdateEntitySaved(user);
            return ResultData(null, b, b ? "移除角色完成！" : "移除角色失败！");
        }


        /// <summary>
        /// 授予临时权限
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="pids">权限id</param>
        /// <returns></returns>
        public ActionResult AddPermissions(Guid id, string pids)
        {
            string[] ids = pids.Split(',');
            var user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "用户不存在");
            }

            List<Permission> permissions = PermissionBll.LoadEntities(p => ids.Contains(p.Id.ToString())).ToList();
            permissions.ForEach(r => { UserPermissionBll.AddEntity(new UserPermission() { HasPermission = true, Permission = r, PermissionId = r.Id, UserInfo = user, UserInfoId = user.Id }); });
            bool b = UserPermissionBll.SaveChanges() > 0;
            return ResultData(null, b, b ? "临时权限配置完成！" : "临时权限配置失败！");
        }

        /// <summary>
        /// 移除临时权限
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="pids">权限id集合</param>
        /// <returns></returns>
        public ActionResult RemovePermissions(Guid id, string pids)
        {
            string[] ids = pids.Split(',');
            UserPermissionBll.DeleteEntitySaved(r => ids.Contains(r.PermissionId.ToString()) && r.UserInfoId.Equals(id));
            return ResultData(null, true, "临时权限配置完成！");
        }

        /// <summary>
        /// 切换临时权限的可用状态
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="pids">权限id集合</param>
        /// <param name="state">可用状态</param>
        /// <returns></returns>
        public ActionResult TogglePermissions(Guid id, string pids, bool state)
        {
            string[] ids = pids.Split(',');
            List<UserPermission> list = UserPermissionBll.LoadEntities(r => ids.Contains(r.PermissionId.ToString()) && r.UserInfoId.Equals(id)).ToList();
            foreach (var p in list)
            {
                p.HasPermission = !state;
            }

            bool b = UserPermissionBll.UpdateEntitiesSaved(list);
            return ResultData(null, b, b ? "临时权限配置完成！" : "临时权限配置失败！");
        }

        #endregion
    }
}