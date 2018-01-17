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
                IList<Control> funs = new List<Control>() { new Control() { Controller = "Home", Action = "Index", HttpMethod = HttpMethod.Get, Name = "��ҳ", ClientApp = apps.FirstOrDefault() }, new Control() { Controller = "User", Action = "Add", HttpMethod = HttpMethod.Post, Name = "����û�", ClientApp = apps.FirstOrDefault() } };
                IList<Permission> ps = new List<Permission>()
                {
                    new Permission() {PermissionName = "��ҳ",Controls = funs.Where(c => c.Controller.Equals("Home")).ToList(), ClientApp = apps},
                    new Permission() {PermissionName = "����˻�",Controls = funs.Where(c => c.Controller.Equals("User")).ToList(), ClientApp = apps},
                    new Permission(){PermissionName = "�����˻�Ȩ��",Controls = funs, ClientApp = apps}
                };
                IList<UserInfo> userInfos = new List<UserInfo>() { new UserInfo() { Username = "admin", Password = "admin".MDString2(salt), SaltKey = salt, Email = "admin@masuit.com", PhoneNumber = "15205201520", ClientApp = apps } };
                IList<Role> roles = new List<Role>() { new Role() { RoleName = "Everyone", Permission = ps, UserInfo = userInfos, ClientApp = apps }, new Role() { RoleName = "Administrator", Permission = ps, UserInfo = userInfos, ClientApp = apps }, new Role() { RoleName = "System", Permission = ps, UserInfo = userInfos, ClientApp = apps } };
                IList<UserGroup> groups = new List<UserGroup>() { new UserGroup() { GroupName = "����Ա", UserInfo = userInfos, ClientApp = apps }, new UserGroup() { GroupName = "��������Ա", UserInfo = userInfos, ClientApp = apps }, new UserGroup() { GroupName = "ϵͳ�ʻ�", UserInfo = userInfos, ClientApp = apps } };
                IList<UserPermission> ups = new List<UserPermission>() { new UserPermission() { Permission = ps.FirstOrDefault(), UserInfo = userInfos.FirstOrDefault(), HasPermission = true } };
                IList<UserGroupRole> ugps = new List<UserGroupRole>() { new UserGroupRole() { Role = roles.FirstOrDefault(), UserGroup = groups.FirstOrDefault(), HasRole = true } };
                context.Permission.AddOrUpdate(p => p.PermissionName, ps.ToArray());
                context.Control.AddOrUpdate(f => new { f.Controller, f.Action }, funs.ToArray());
                context.Role.AddOrUpdate(r => r.RoleName, roles.ToArray());
                context.UserGroup.AddOrUpdate(g => g.GroupName, groups.ToArray());
                context.UserInfo.AddOrUpdate(u => u.Username, userInfos.ToArray());
                context.UserPermission.AddOrUpdate(p => new { p.PermissionId, p.UserInfoId }, ups.ToArray());
                context.UserGroupPermission.AddOrUpdate(p => new { p.RoleId, p.UserGroupId }, ugps.ToArray());
                context.SaveChanges();
                context.Database.ExecuteSqlCommand(@"Create PROC [dbo].[sp_getChildrenMenuByParentId](@ParentId int)
                                                                    AS
                                                                    BEGIN    
                                                                        WITH Tree
                                                                            AS (SELECT * FROM Menu WHERE Id = @ParentId  --��һ����ѯ��Ϊ�ݹ�Ļ���(ê��)
                                                                                UNION ALL
                                                                                SELECT Menu.*     --�ڶ�����ѯ��Ϊ�ݹ��Ա�� �����Ա�Ľ��Ϊ��ʱ���˵ݹ������
                                                                                  FROM Tree INNER JOIN Menu ON Tree.Id = Menu.ParentId) 
                                                                            SELECT * FROM Tree   
                                                                    END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROCEDURE [dbo].[sp_getParentMenuIdByChildId]
                                                                          @cid AS int =1 
                                                                        AS
                                                                        BEGIN
	                                                                        WITH Tree
                                                                         AS (SELECT * FROM Menu WHERE Id = @cid  --��һ����ѯ��Ϊ�ݹ�Ļ���(ê��)
                                                                             UNION ALL
                                                                             SELECT Menu.*     --�ڶ�����ѯ��Ϊ�ݹ��Ա�� �����Ա�Ľ��Ϊ��ʱ���˵ݹ������
                                                                               FROM Tree INNER JOIN Menu ON Tree.ParentId = Menu.Id) 
                                                                         SELECT top 1 Id FROM Tree  ORDER BY Id
                                                                        END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROC [dbo].[sp_getChildrenGroupByParentId](@ParentId int)
                                                                    AS
                                                                    BEGIN    
                                                                        WITH Tree
                                                                            AS (SELECT * FROM UserGroup WHERE Id = @ParentId  --��һ����ѯ��Ϊ�ݹ�Ļ���(ê��)
                                                                                UNION ALL
                                                                                SELECT UserGroup.*     --�ڶ�����ѯ��Ϊ�ݹ��Ա�� �����Ա�Ľ��Ϊ��ʱ���˵ݹ������
                                                                                  FROM Tree INNER JOIN UserGroup ON Tree.Id = UserGroup.ParentId) 
                                                                            SELECT * FROM Tree   
                                                                    END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROCEDURE [dbo].[sp_getParentGroupIdByChildId]
                                                                          @cid AS int =1 
                                                                        AS
                                                                        BEGIN
	                                                                        WITH Tree
                                                                         AS (SELECT * FROM UserGroup WHERE Id = @cid  --��һ����ѯ��Ϊ�ݹ�Ļ���(ê��)
                                                                             UNION ALL
                                                                             SELECT UserGroup.*     --�ڶ�����ѯ��Ϊ�ݹ��Ա�� �����Ա�Ľ��Ϊ��ʱ���˵ݹ������
                                                                               FROM Tree INNER JOIN UserGroup ON Tree.ParentId = UserGroup.Id) 
                                                                         SELECT top 1 Id FROM Tree  ORDER BY Id
                                                                        END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROC [dbo].[sp_getChildrenRoleByParentId](@ParentId int)
                                                                    AS
                                                                    BEGIN    
                                                                        WITH Tree
                                                                            AS (SELECT * FROM Role WHERE Id = @ParentId  --��һ����ѯ��Ϊ�ݹ�Ļ���(ê��)
                                                                                UNION ALL
                                                                                SELECT Role.*     --�ڶ�����ѯ��Ϊ�ݹ��Ա�� �����Ա�Ľ��Ϊ��ʱ���˵ݹ������
                                                                                  FROM Tree INNER JOIN Role ON Tree.Id = Role.ParentId) 
                                                                            SELECT * FROM Tree   
                                                                    END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROCEDURE [dbo].[sp_getParentRoleIdByChildId]
                                                                          @cid AS int =1 
                                                                        AS
                                                                        BEGIN
	                                                                        WITH Tree
                                                                         AS (SELECT * FROM Role WHERE Id = @cid  --��һ����ѯ��Ϊ�ݹ�Ļ���(ê��)
                                                                             UNION ALL
                                                                             SELECT Role.*     --�ڶ�����ѯ��Ϊ�ݹ��Ա�� �����Ա�Ľ��Ϊ��ʱ���˵ݹ������
                                                                               FROM Tree INNER JOIN Role ON Tree.ParentId = Role.Id) 
                                                                         SELECT top 1 Id FROM Tree  ORDER BY Id
                                                                        END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROC [dbo].[sp_getChildrenPermissionByParentId](@ParentId int)
                                                                    AS
                                                                    BEGIN    
                                                                        WITH Tree
                                                                            AS (SELECT * FROM Permission WHERE Id = @ParentId  --��һ����ѯ��Ϊ�ݹ�Ļ���(ê��)
                                                                                UNION ALL
                                                                                SELECT Permission.*     --�ڶ�����ѯ��Ϊ�ݹ��Ա�� �����Ա�Ľ��Ϊ��ʱ���˵ݹ������
                                                                                  FROM Tree INNER JOIN Permission ON Tree.Id = Permission.ParentId) 
                                                                            SELECT * FROM Tree   
                                                                    END
                                                                    ");
                context.Database.ExecuteSqlCommand(@"Create PROCEDURE [dbo].[sp_getParentPermissionIdByChildId]
                                                                          @cid AS int =1 
                                                                        AS
                                                                        BEGIN
	                                                                        WITH Tree
                                                                         AS (SELECT * FROM Permission WHERE Id = @cid  --��һ����ѯ��Ϊ�ݹ�Ļ���(ê��)
                                                                             UNION ALL
                                                                             SELECT Permission.*     --�ڶ�����ѯ��Ϊ�ݹ��Ա�� �����Ա�Ľ��Ϊ��ʱ���˵ݹ������
                                                                               FROM Tree INNER JOIN Permission ON Tree.ParentId = Permission.Id) 
                                                                         SELECT top 1 Id FROM Tree  ORDER BY Id
                                                                        END
                                                                    ");
            }
#endif

        }
    }
}