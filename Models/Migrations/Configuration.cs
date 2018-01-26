using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
                //初始化客户端子系统
                string appid = Guid.NewGuid().ToString().MDString();
                List<ClientApp> apps = new List<ClientApp>() { new ClientApp() { AppName = "LocalSystem", AppId = "e51be06c55248fd0165873467ceaf256", AppSecret = appid.MDString(DateTime.Now.GetTotalMilliseconds().ToString()), Description = "主系统应用程序", Preset = true, Available = true } };

                //初始化功能控制
                IList<Control> controls = new List<Control>();
                Regex regex = new Regex(@"(\w+)Controller\.(\w+)\S*"">\r\n\s*<summary>\r\n\s*(\w+)\r\n\s*</summary>");
                var docpath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory).Any(s => s.Contains("Web.config")) ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "bin", "SSO.Passport.IdentityServer.xml") : Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "SSO.Passport.IdentityServer", "bin", "SSO.Passport.IdentityServer.xml");

                string text = File.ReadAllText(docpath);
                MatchCollection mc = regex.Matches(text);
                foreach (Match m in mc)
                {
                    controls.Add(new Control() { Controller = m.Groups[1].Value, Action = m.Groups[2].Value, Name = m.Groups[3].Value, ClientApp = apps.FirstOrDefault(), HttpMethod = HttpMethod.Post });
                }

                var childmenu = new Menu() { Name = "查看系统日志", RouteName = "system-log", Route = "system-log", ClientApp = apps.FirstOrDefault(), Sort = 900 };
                //初始化菜单
                List<Menu> menus = new List<Menu>()
                {
                    new Menu(){Name = "控制面板",CssStyle = "zmdi-home",RouteName = "dashboard",Route = "dashboard",ClientApp = apps.FirstOrDefault(),Sort = 10},
                    new Menu(){Name = "子系统管理",CssStyle = "zmdi-card-travel",RouteName = "apps",Route = "apps",ClientApp = apps.FirstOrDefault(),Sort = 20},
                    new Menu(){Name = "用户管理",CssStyle = "zmdi-assignment-account",RouteName = "user",Route = "user",ClientApp = apps.FirstOrDefault(),Sort = 30},
                    new Menu(){Name = "用户组管理",CssStyle = "zmdi-flower-alt",RouteName = "group",Route = "group",ClientApp = apps.FirstOrDefault(),Sort = 40},
                    new Menu(){Name = "角色管理",CssStyle = "zmdi-collection-text",RouteName = "role",Route = "role",ClientApp = apps.FirstOrDefault(),Sort = 50},
                    new Menu(){Name = "权限管理",CssStyle = "zmdi-globe-lock",RouteName = "permission",Route = "permission",ClientApp = apps.FirstOrDefault(),Sort = 60},
                    new Menu(){Name = "访问控制管理",CssStyle = "zmdi-flash-auto",RouteName = "access",Route = "access",ClientApp = apps.FirstOrDefault(),Sort = 70},
                    new Menu(){Name = "菜单管理",CssStyle = "zmdi-menu",RouteName = "menu",Route = "menu",ClientApp = apps.FirstOrDefault(),Sort = 80},
                    new Menu(){Name = "系统设置",CssStyle = "zmdi-wrench",RouteName = "system",Route = "system",ClientApp = apps.FirstOrDefault(),Sort = 90,Children = new List<Menu>(){childmenu}},
                    new Menu(){Name = "文件管理",CssStyle = "zmdi-archive",RouteName = "filemanager",Route = "filemanager",ClientApp = apps.FirstOrDefault(),Sort = 100},
                    new Menu(){Name = "任务管理器",CssStyle = "zmdi-view-list",RouteName = "taskcenter",Route = "taskcenter",ClientApp = apps.FirstOrDefault(),Sort = 110},
                    new Menu(){Name = "swagger",CssStyle = "zmdi-code-setting",RouteName = "swagger",Route = "swagger",ClientApp = apps.FirstOrDefault(),Sort = 120}
                };

                //初始化权限
                var settingsMenu = menus.Where(m => m.Name.Equals("系统设置")).ToList();
                settingsMenu.Add(childmenu);
                IList<Permission> ps = new List<Permission>()
                {
                    new Permission() {PermissionName = "超级权限",Controls = controls,Menu = menus,ClientApp = apps,Description = "超级权限"},
                    new Permission() {PermissionName = "主系统首页",Controls = controls.Where(c => c.Controller.Equals("Home")).ToList(),Menu = menus.Where(m => m.Name.Equals("控制面板")).ToList(), ClientApp = apps,Description = "主系统首页控制"},
                    new Permission() {PermissionName = "主系统功能访问控制",Controls = controls.Where(c => c.Controller.Equals("Access")).ToList(),Menu = menus.Where(m => m.Name.Equals("访问控制管理")).ToList(), ClientApp = apps,Description = "主系统功能访问控制"},
                    new Permission() {PermissionName = "主系统菜单访问控制",Controls = controls.Where(c => c.Controller.Equals("Menu")).ToList(),Menu = menus.Where(m => m.Name.Equals("菜单管理")).ToList(), ClientApp = apps,Description = "主系统菜单访问控制"},
                    new Permission() {PermissionName = "子系统管理",Controls = controls.Where(c => c.Controller.Equals("App")).ToList(),Menu = menus.Where(m => m.Name.Equals("子系统管理")).ToList(), ClientApp = apps,Description = "子系统管理"},
                    new Permission() {PermissionName = "主系统文件管理",Controls = controls.Where(c => c.Controller.Equals("File")).ToList(),Menu = menus.Where(m => m.Name.Equals("文件管理")).ToList(), ClientApp = apps,Description = "主系统文件管理"},
                    new Permission() {PermissionName = "主系统用户组管理",Controls = controls.Where(c => c.Controller.Equals("Group")).ToList(),Menu = menus.Where(m => m.Name.Equals("用户组管理")).ToList(), ClientApp = apps,Description = "主系统用户组管理"},
                    new Permission() {PermissionName = "主系统权限管理",Controls = controls.Where(c => c.Controller.Equals("Permission")).ToList(),Menu = menus.Where(m => m.Name.Equals("权限管理")).ToList(), ClientApp = apps,Description = "主系统权限管理"},
                    new Permission() {PermissionName = "主系统角色管理",Controls = controls.Where(c => c.Controller.Equals("Role")).ToList(),Menu = menus.Where(m => m.Name.Equals("角色管理")).ToList(), ClientApp = apps,Description = "主系统角色管理"},
                    new Permission() {PermissionName = "主系统系统设置",Controls = controls.Where(c => c.Controller.Equals("System")).ToList(),Menu = settingsMenu, ClientApp = apps,Description = "主系统系统设置"},
                    new Permission() {PermissionName = "主系统用户管理",Controls = controls.Where(c => c.Controller.Equals("User")).ToList(),Menu = menus.Where(m => m.Name.Equals("用户管理")).ToList(), ClientApp = apps,Description = "主系统用户管理"},
                };

                //初始化用户
                var salt = $"{new Random().StrictNext()}{DateTime.Now.GetTotalMilliseconds()}".MDString2(Guid.NewGuid().ToString()).Base64Encrypt();
                IList<UserInfo> userInfos = new List<UserInfo>()
                {
                    new UserInfo() { Username = "admin", Password = "admin".MDString2(salt), SaltKey = salt, Email = "admin@masuit.com", PhoneNumber = "15205201520", ClientApp = apps,AccessKey = Guid.NewGuid().ToString().MDString(),IsMaster = true,IsPreset = true,Avatar = "/Assets/logo.png"}
                };

                //初始化角色
                IList<Role> roles = new List<Role>()
                {
                    new Role() { RoleName = "Administrator", Permission = ps, UserInfo = userInfos, ClientApp = apps },
                    new Role() { RoleName = "System", Permission = ps, UserInfo = userInfos, ClientApp = apps },
                    new Role() { RoleName = "Test"},
                    new Role() { RoleName = "Demo"},
                };

                //初始化用户组
                IList<UserGroup> groups = new List<UserGroup>()
                {
                    new UserGroup() { GroupName = "管理组", UserInfo = userInfos, ClientApp = apps },
                    new UserGroup() { GroupName = "管理员", UserInfo = userInfos, ClientApp = apps },
                    new UserGroup() { GroupName = "超级管理员", UserInfo = userInfos, ClientApp = apps },
                    new UserGroup() { GroupName = "系统用户", UserInfo = userInfos, ClientApp = apps },
                    new UserGroup() { GroupName = "Test"},
                };
                IList<UserPermission> ups = new List<UserPermission>() { new UserPermission() { Permission = ps.FirstOrDefault(), UserInfo = userInfos.FirstOrDefault(), HasPermission = true } };
                IList<UserGroupRole> ugps = new List<UserGroupRole>() { new UserGroupRole() { Role = roles.FirstOrDefault(), UserGroup = groups.FirstOrDefault(), HasRole = true } };
                context.UserGroup.AddOrUpdate(g => g.GroupName, groups.ToArray());
                context.Role.AddOrUpdate(g => g.RoleName, roles.ToArray());
                context.UserPermission.AddOrUpdate(p => new { p.PermissionId, p.UserInfoId }, ups.ToArray());
                context.UserGroupPermission.AddOrUpdate(p => new { p.RoleId, p.UserGroupId }, ugps.ToArray());
                context.SaveChanges();
                context.Database.ExecuteSqlCommand(@"Create PROC [dbo].[sp_getChildrenMenuByParentId](@ParentId int)
                                                                    AS
                                                                    BEGIN    
                                                                        WITH Tree
                                                                            AS (SELECT * FROM Menu WHERE Id = @ParentId  --第一个查询作为递归的基点(锚点)
                                                                                UNION ALL
                                                                                SELECT Menu.*    --第二个查询作为递归成员， 下属成员的结果为空时，此递归结束。
                                                                                  FROM Tree INNER JOIN Menu ON Tree.Id = Menu.ParentId) 
                                                                            SELECT * FROM Tree   
                                                                    END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROCEDURE [dbo].[sp_getParentMenuIdByChildId]
                                                                          @cid AS int =1 
                                                                        AS
                                                                        BEGIN
	                                                                        WITH Tree
                                                                         AS (SELECT * FROM Menu WHERE Id = @cid --第一个查询作为递归的基点(锚点)
                                                                             UNION ALL
                                                                             SELECT Menu.*    --第二个查询作为递归成员， 下属成员的结果为空时，此递归结束。
                                                                               FROM Tree INNER JOIN Menu ON Tree.ParentId = Menu.Id) 
                                                                         SELECT Id FROM Tree  ORDER BY Id
                                                                        END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROC [dbo].[sp_getChildrenGroupByParentId](@ParentId int)
                                                                    AS
                                                                    BEGIN    
                                                                        WITH Tree
                                                                            AS (SELECT * FROM UserGroup WHERE Id = @ParentId --第一个查询作为递归的基点(锚点)
                                                                                UNION ALL
                                                                                SELECT UserGroup.*    --第二个查询作为递归成员， 下属成员的结果为空时，此递归结束。
                                                                                  FROM Tree INNER JOIN UserGroup ON Tree.Id = UserGroup.ParentId) 
                                                                            SELECT * FROM Tree   
                                                                    END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROCEDURE [dbo].[sp_getParentGroupIdByChildId]
                                                                          @cid AS int =1 
                                                                        AS
                                                                        BEGIN
	                                                                        WITH Tree
                                                                         AS (SELECT * FROM UserGroup WHERE Id = @cid --第一个查询作为递归的基点(锚点)
                                                                             UNION ALL
                                                                             SELECT UserGroup.*    --第二个查询作为递归成员， 下属成员的结果为空时，此递归结束。
                                                                               FROM Tree INNER JOIN UserGroup ON Tree.ParentId = UserGroup.Id) 
                                                                         SELECT Id FROM Tree  ORDER BY Id
                                                                        END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROC [dbo].[sp_getChildrenRoleByParentId](@ParentId int)
                                                                    AS
                                                                    BEGIN    
                                                                        WITH Tree
                                                                            AS (SELECT * FROM Role WHERE Id = @ParentId --第一个查询作为递归的基点(锚点)
                                                                                UNION ALL
                                                                                SELECT Role.*    --第二个查询作为递归成员， 下属成员的结果为空时，此递归结束。
                                                                                  FROM Tree INNER JOIN Role ON Tree.Id = Role.ParentId) 
                                                                            SELECT * FROM Tree   
                                                                    END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROCEDURE [dbo].[sp_getParentRoleIdByChildId]
                                                                          @cid AS int =1 
                                                                        AS
                                                                        BEGIN
	                                                                        WITH Tree
                                                                         AS (SELECT * FROM Role WHERE Id = @cid --第一个查询作为递归的基点(锚点)
                                                                             UNION ALL
                                                                             SELECT Role.*    --第二个查询作为递归成员， 下属成员的结果为空时，此递归结束。
                                                                               FROM Tree INNER JOIN Role ON Tree.ParentId = Role.Id) 
                                                                         SELECT Id FROM Tree  ORDER BY Id
                                                                        END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROC [dbo].[sp_getChildrenPermissionByParentId](@ParentId int)
                                                                    AS
                                                                    BEGIN    
                                                                        WITH Tree
                                                                            AS (SELECT * FROM Permission WHERE Id = @ParentId --第一个查询作为递归的基点(锚点)
                                                                                UNION ALL
                                                                                SELECT Permission.*    --第二个查询作为递归成员， 下属成员的结果为空时，此递归结束。
                                                                                  FROM Tree INNER JOIN Permission ON Tree.Id = Permission.ParentId) 
                                                                            SELECT * FROM Tree   
                                                                    END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROCEDURE [dbo].[sp_getParentPermissionIdByChildId]
                                                                          @cid AS int =1 
                                                                        AS
                                                                        BEGIN
	                                                                        WITH Tree
                                                                         AS (SELECT * FROM Permission WHERE Id = @cid --第一个查询作为递归的基点(锚点)
                                                                             UNION ALL
                                                                             SELECT Permission.*    --第二个查询作为递归成员， 下属成员的结果为空时，此递归结束。
                                                                               FROM Tree INNER JOIN Permission ON Tree.ParentId = Permission.Id) 
                                                                         SELECT Id FROM Tree  ORDER BY Id
                                                                        END
                                                                    ");
            }
#endif

        }
    }
}