using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Security;
using Masuit.Tools.Win32;
using Models.Application;
using Models.Entity;
using Models.Enum;

namespace Models.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataContext context)
        {
#if DEBUG
            //var salt = $"{new Random().StrictNext()}{DateTime.Now.GetTotalMilliseconds()}".MDString2(Guid.NewGuid().ToString()).Base64Encrypt();
            //IList<Function> funs = new List<Function>() { new Function() { Controller = "Home", Action = "Index", HttpMethod = "Get", FunctionType = FunctionType.Menu, PermissionId = 1, CssStyle = "icon-book", IconUrl = "http://www.baidu.com/favicon.ico" }, new Function() { Controller = "User", Action = "Add", FunctionType = FunctionType.Operating, PermissionId = 1, HttpMethod = "Post" } };
            //IList<Permission> ps = new List<Permission>()
            //{
            //    new Permission() {PermissionName = "首页",Function = funs.Where(c => c.Controller.Equals("Home")).ToList()},
            //    new Permission() {PermissionName = "添加账户",Function = funs.Where(c => c.Controller.Equals("User")).ToList()},
            //    new Permission(){PermissionName = "本地账户权限",Function = funs}
            //};
            //IList<UserInfo> userInfos = new List<UserInfo>() { new UserInfo() { Username = "admin", Password = "admin".MDString2(salt), SaltKey = salt, Email = "admin@masuit.com", PhoneNumber = "15205201520" } };
            //IList<Role> roles = new List<Role>() { new Role() { RoleName = "Everyone", Permission = ps, UserInfo = userInfos }, new Role() { RoleName = "Administrator", Permission = ps, UserInfo = userInfos }, new Role() { RoleName = "System", Permission = ps, UserInfo = userInfos } };
            //IList<UserGroup> groups = new List<UserGroup>() { new UserGroup() { GroupName = "管理员", UserInfo = userInfos }, new UserGroup() { GroupName = "超级管理员", UserInfo = userInfos }, new UserGroup() { GroupName = "系统帐户", UserInfo = userInfos } };
            //IList<UserPermission> ups = new List<UserPermission>() { new UserPermission() { Permission = ps.FirstOrDefault(), UserInfo = userInfos.FirstOrDefault(), HasPermission = true } };
            //IList<UserGroupPermission> ugps = new List<UserGroupPermission>() { new UserGroupPermission() { Role = roles.FirstOrDefault(), UserGroup = groups.FirstOrDefault(), HasPermission = true } };

            //context.Permission.AddOrUpdate(p => p.PermissionName, ps.ToArray());
            //context.Function.AddOrUpdate(f => new { f.Controller, f.Action }, funs.ToArray());
            //context.Role.AddOrUpdate(r => r.RoleName, roles.ToArray());
            //context.UserGroup.AddOrUpdate(g => g.GroupName, groups.ToArray());
            //context.UserInfo.AddOrUpdate(u => u.Username, userInfos.ToArray());
            //context.UserPermission.AddOrUpdate(p => new { p.PermissionId, p.UserInfoId }, ups.ToArray());
            //context.UserGroupPermission.AddOrUpdate(p => new { p.RoleId, p.UserGroupId }, ugps.ToArray());
            //context.SaveChanges();
#endif

        }
    }
}