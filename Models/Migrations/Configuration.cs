using System;
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
    internal sealed class Configuration : DbMigrationsConfiguration<PermissionContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(PermissionContext context)
        {
#if DEBUG
            var salt = $"{new Random().StrictNext()}{DateTime.Now.GetTotalMilliseconds()}".MDString2(Guid.NewGuid().ToString()).Base64Encrypt();
            UserInfo userInfo = new UserInfo() { Username = "admin", Password = "admin".MDString2(salt), SaltKey = salt, Email = "admin@masuit.com", PhoneNumber = "15205201520" };
            UserGroup @group = new UserGroup() { GroupName = "管理员" };
            Role role = new Role() { RoleName = "Everyone" };
            Permission permission = new Permission() { PermissionName = "首页" };

            context.UserInfo.AddOrUpdate(u => u.Username, userInfo);
            context.SaveChanges();

            context.UserGroup.AddOrUpdate(g => g.GroupName, group, new UserGroup() { GroupName = "超级管理员" }, new UserGroup() { GroupName = "系统帐户" });
            context.SaveChanges();

            context.Role.AddOrUpdate(r => r.RoleName, role, new Role() { RoleName = "Administrator" }, new Role() { RoleName = "System" });
            context.SaveChanges();

            if (!context.Role.Find(1).UserInfo.Any())
            {
                context.Role.Find(1).UserInfo.Add(userInfo);
            }
            context.Permission.AddOrUpdate(p => p.PermissionName, permission, new Permission() { PermissionName = "添加账户" });
            context.SaveChanges();

            if (!context.Permission.Find(1).Role.Any())
            {
                context.Permission.Find(1).Role.Add(role);
            }
            context.Function.AddOrUpdate(f => new { f.Controller, f.Action }, new Function() { Controller = "Home", Action = "Index", HttpMethod = "Get", FunctionType = FunctionType.Menu, PermissionId = 1, CssStyle = "icon-book", IconUrl = "http://www.baidu.com/favicon.ico" }, new Function() { Controller = "User", Action = "Add", FunctionType = FunctionType.Operating, PermissionId = 1, HttpMethod = "Post" });
            if (!context.UserGroup.Find(1).UserInfo.Any())
            {
                context.UserGroup.Find(1).UserInfo.Add(userInfo);
            }
            context.UserGroupPermission.AddOrUpdate(p => new { p.UserGroupId, p.RoleId }, new UserGroupPermission() { UserGroupId = group.Id, RoleId = role.Id });
            context.SaveChanges();

            context.UserPermission.AddOrUpdate(p => new { p.UserInfoId, p.PermissionId }, new UserPermission() { PermissionId = permission.Id, UserInfoId = userInfo.Id });
            context.SaveChanges();
#endif
        }
    }
}