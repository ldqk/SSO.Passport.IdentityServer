using System;
using System.Collections.Generic;
using BLL;
using Models.Entity;

namespace InitDataSource
{
    class Program
    {
        static void Main(string[] args)
        {
            UserInfoBll service = new UserInfoBll();
            UserInfo userInfo = service.Register(new UserInfo() { Username = "admin", Password = "admin", Email = "admin@masuit.com", PhoneNumber = "15205201520" });
            Console.WriteLine(userInfo.Id);
            UserGroupBll groupBll = new UserGroupBll();
            groupBll.AddEntities(new List<UserGroup>() { new UserGroup() { GroupName = "管理员" }, new UserGroup() { GroupName = "超级管理员" }, new UserGroup() { GroupName = "系统帐户" } });
            RoleBll roleBll = new RoleBll();
            roleBll.AddEntities(new List<Role>() { new Role() { RoleName = "Everyone" }, new Role() { RoleName = "Administrator" }, new Role() { RoleName = "System" } });
            PermissionBll permissionBll = new PermissionBll();
            permissionBll.AddEntities(new List<Permission>() { new Permission() { PermissionName = "首页" }, new Permission() { PermissionName = "添加账户" } });
            permissionBll.SaveChangesAsync();
            Console.ReadKey();
        }
    }
}
