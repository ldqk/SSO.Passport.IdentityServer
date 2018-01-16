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
            if (!context.UserInfo.Any())
            {
                string appid = Guid.NewGuid().ToString().MDString();
                List<ClientApp> apps = new List<ClientApp>() { new ClientApp() { AppName = "LocalSystem", AppId = appid, AppSecret = appid.MDString(DateTime.Now.GetTotalMilliseconds().ToString()) } };
                var salt = $"{new Random().StrictNext()}{DateTime.Now.GetTotalMilliseconds()}".MDString2(Guid.NewGuid().ToString()).Base64Encrypt();
                IList<Control> funs = new List<Control>() { new Control() { Controller = "Home", Action = "Index", HttpMethod = HttpMethod.Get, Name = "首页", ClientApp = apps.FirstOrDefault() }, new Control() { Controller = "User", Action = "Add", HttpMethod = HttpMethod.Post, Name = "添加用户", ClientApp = apps.FirstOrDefault() } };
                IList<Permission> ps = new List<Permission>()
                {
                    new Permission() {PermissionName = "首页",Controls = funs.Where(c => c.Controller.Equals("Home")).ToList(), ClientApp = apps.FirstOrDefault()},
                    new Permission() {PermissionName = "添加账户",Controls = funs.Where(c => c.Controller.Equals("User")).ToList(), ClientApp = apps.FirstOrDefault()},
                    new Permission(){PermissionName = "本地账户权限",Controls = funs, ClientApp = apps.FirstOrDefault()}
                };
                IList<UserInfo> userInfos = new List<UserInfo>() { new UserInfo() { Username = "admin", Password = "admin".MDString2(salt), SaltKey = salt, Email = "admin@masuit.com", PhoneNumber = "15205201520", ClientApp = apps.FirstOrDefault() } };
                IList<Role> roles = new List<Role>() { new Role() { RoleName = "Everyone", Permission = ps, UserInfo = userInfos, ClientApp = apps.FirstOrDefault() }, new Role() { RoleName = "Administrator", Permission = ps, UserInfo = userInfos, ClientApp = apps.FirstOrDefault() }, new Role() { RoleName = "System", Permission = ps, UserInfo = userInfos, ClientApp = apps.FirstOrDefault() } };
                IList<UserGroup> groups = new List<UserGroup>() { new UserGroup() { GroupName = "管理员", UserInfo = userInfos, ClientApp = apps.FirstOrDefault() }, new UserGroup() { GroupName = "超级管理员", UserInfo = userInfos, ClientApp = apps.FirstOrDefault() }, new UserGroup() { GroupName = "系统帐户", UserInfo = userInfos, ClientApp = apps.FirstOrDefault() } };
                IList<UserPermission> ups = new List<UserPermission>() { new UserPermission() { Permission = ps.FirstOrDefault(), UserInfo = userInfos.FirstOrDefault(), HasPermission = true } };
                IList<UserGroupPermission> ugps = new List<UserGroupPermission>() { new UserGroupPermission() { Role = roles.FirstOrDefault(), UserGroup = groups.FirstOrDefault(), HasPermission = true } };
                context.Permission.AddOrUpdate(p => p.PermissionName, ps.ToArray());
                context.Control.AddOrUpdate(f => new { f.Controller, f.Action }, funs.ToArray());
                context.Role.AddOrUpdate(r => r.RoleName, roles.ToArray());
                context.UserGroup.AddOrUpdate(g => g.GroupName, groups.ToArray());
                context.UserInfo.AddOrUpdate(u => u.Username, userInfos.ToArray());
                context.UserPermission.AddOrUpdate(p => new { p.PermissionId, p.UserInfoId }, ups.ToArray());
                context.UserGroupPermission.AddOrUpdate(p => new { p.RoleId, p.UserGroupId }, ugps.ToArray());
                context.SaveChanges();
            }
#endif

        }
    }
}