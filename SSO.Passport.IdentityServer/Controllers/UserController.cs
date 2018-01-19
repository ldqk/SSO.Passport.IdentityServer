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
            List<UserInfoDto> list = UserInfoBll.LoadPageEntitiesFromL2CacheNoTracking<DateTime, UserInfoDto>(page, size, out int total, where, u => u.LastLoginTime, false).ToList();
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
        #region 权限配置

        /// <summary>
        /// 分配给客户端子系统
        /// </summary>
        /// <param name="id"></param>
        /// <param name="aids"></param>
        /// <returns></returns>
        public ActionResult AddApps(Guid id, string aids)
        {
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<ClientApp> apps = ClientAppBll.LoadEntities(a => aids.Contains(a.Id.ToString())).ToList();
            apps.ForEach(a => user.ClientApp.Add(a));
            UserInfoBll.UpdateEntity(user);
            UserInfoBll.BulkSaveChanges();
            return ResultData(null, true, "分配给客户端子系统完成！");
        }

        /// <summary>
        /// 添加到用户组
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="gids">用户组id</param>
        /// <returns></returns>
        public ActionResult AddGroups(Guid id, string gids)
        {
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<UserGroup> groups = UserGroupBll.LoadEntities(g => gids.Contains(g.Id.ToString())).ToList();
            groups.ForEach(g => user.UserGroup.Add(g));
            UserInfoBll.UpdateEntity(user);
            UserInfoBll.BulkSaveChanges();
            return ResultData(null, true, "分配到用户组完成！");
        }

        /// <summary>
        /// 移除用户组
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="gids">用户组id</param>
        /// <returns></returns>
        public ActionResult RemoveGroups(Guid id, string gids)
        {
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<UserGroup> groups = UserGroupBll.LoadEntities(g => gids.Contains(g.Id.ToString())).ToList();
            groups.ForEach(g => user.UserGroup.Remove(g));
            UserInfoBll.UpdateEntity(user);
            UserInfoBll.BulkSaveChanges();
            return ResultData(null, true, "移除用户组完成！");
        }

        /// <summary>
        /// 添加到角色
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="rids">角色id</param>
        /// <returns></returns>
        public ActionResult AddRoles(Guid id, string rids)
        {
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<Role> roles = RoleBll.LoadEntities(r => rids.Contains(r.Id.ToString())).ToList();
            roles.ForEach(r => user.Role.Add(r));
            UserInfoBll.UpdateEntity(user);
            UserInfoBll.BulkSaveChanges();
            return ResultData(null, true, "分配角色完成！");
        }

        /// <summary>
        /// 移除角色
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="rids">角色id</param>
        /// <returns></returns>
        public ActionResult RemoveRoles(Guid id, string rids)
        {
            UserInfo user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "未找到用户！");
            }

            List<Role> groups = RoleBll.LoadEntities(r => rids.Contains(r.Id.ToString())).ToList();
            groups.ForEach(r => user.Role.Remove(r));
            UserInfoBll.UpdateEntity(user);
            UserInfoBll.BulkSaveChanges();
            return ResultData(null, true, "移除角色完成！");
        }


        /// <summary>
        /// 授予临时权限
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="pids">权限id</param>
        /// <returns></returns>
        public ActionResult AddPermissions(Guid id, string pids)
        {
            var user = UserInfoBll.GetById(id);
            if (user is null)
            {
                return ResultData(null, false, "用户不存在");
            }

            List<Permission> permissions = PermissionBll.LoadEntities(p => pids.Contains(p.Id.ToString())).ToList();
            permissions.ForEach(r =>
            {
                UserPermissionBll.AddEntity(new UserPermission() { HasPermission = true, Permission = r, PermissionId = r.Id, UserInfo = user, UserInfoId = user.Id });
            });
            UserPermissionBll.BulkSaveChanges();
            return ResultData(null, true, "临时权限配置完成！");
        }

        /// <summary>
        /// 移除临时权限
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="pids">权限id集合</param>
        /// <returns></returns>
        public ActionResult RemovePermissions(Guid id, string pids)
        {
            bool b = UserPermissionBll.DeleteEntitySaved(r => pids.Contains(r.PermissionId.ToString()) && r.UserInfoId.Equals(id)) > 0;
            return ResultData(null, b, b ? "临时权限配置完成！" : "临时权限配置失败！");
        }
        #endregion
    }
}