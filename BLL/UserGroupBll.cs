using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Masuit.Tools;
using Models.Application;
using Models.Dto;
using Models.Entity;

namespace BLL
{
    public partial class UserGroupBll
    {
        /// <summary>
        /// 根据名称找权限
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UserGroup GetGroupByName(string name)
        {
            return GetFirstEntity(g => g.GroupName.Equals(name));
        }

        /// <summary>
        /// 判断权限名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GroupNameExist(string name)
        {
            return GetGroupByName(name) != null;
        }

        /// <summary>
        /// 根据权限名找所属的用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<UserInfo> GetUserInfoList(string name)
        {
            return GetGroupByName(name).UserInfo;
        }

        /// <summary>
        /// 通过存储过程获得自己以及自己所有的子元素集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DbRawSqlQuery<UserGroupOutputDto> GetSelfAndChildrenByParentId(int id)
        {
            return BaseDal.GetDataContext().Database.SqlQuery<UserGroupOutputDto>("exec sp_getChildrenGroupByParentId " + id);
        }

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<int> GetParentIdById(int id)
        {
            return BaseDal.GetDataContext().Database.SqlQuery<int>("exec sp_getParentGroupIdByChildId " + id).ToList();
        }

        /// <summary>
        /// 获取用户组所有的访问控制详情
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public (List<ClientApp>, List<UserInfo>, List<UserGroup>, List<Role>, List<Permission>, List<Control>, List<Menu>) Details(UserGroup g)
        {
            DataContext context = BaseDal.GetDataContext();
            List<ClientApp> apps = g.ClientApp.Distinct().ToList();
            List<UserInfo> users = g.UserInfo.Distinct().ToList();
            List<UserGroup> groups = new List<UserGroup>();
            List<Control> controls = new List<Control>();
            List<Menu> menus = new List<Menu>();
            List<Role> roles = new List<Role>();
            List<Permission> permissions = new List<Permission>();
            //2.1 拿到所有上级用户组
            int[] gids = context.Database.SqlQuery<int>("exec sp_getParentGroupIdByChildId " + g.Id).ToArray(); //拿到所有上级用户组
            foreach (int i in gids)
            {
                UserGroup group = context.UserGroup.FirstOrDefault(u => u.Id == i);
                if (i != g.Id)
                {
                    groups.Add(group);
                }
                List<int> noRoleIds = @group?.UserGroupPermission.Where(x => !x.HasRole).Select(x => x.Id).ToList(); //没有角色的id集合
                @group?.UserGroupPermission.ForEach(ugp =>
                {
                    if (ugp.HasRole)
                    {
                        //角色可用，取并集
                        //2.2 拿到所有上级角色，并排除掉角色不可用的角色id
                        int[] rids = context.Database.SqlQuery<int>("exec sp_getParentRoleIdByChildId " + ugp.Role.Id).Except(noRoleIds).ToArray(); //拿到所有上级角色，并排除掉角色不可用的角色id
                        foreach (int r in rids)
                        {
                            Role role = context.Role.FirstOrDefault(o => o.Id == r);
                            roles.Add(role);
                            role?.Permission.ForEach(p =>
                            {
                                //2.3 拿到所有上级权限
                                int[] pids = context.Database.SqlQuery<int>("exec sp_getParentPermissionIdByChildId " + p.Id).ToArray(); //拿到所有上级权限
                                foreach (int s in pids)
                                {
                                    Permission permission = context.Permission.FirstOrDefault(x => x.Id == s);
                                    permissions.Add(permission);
                                    controls.AddRange(permission.Controls.Where(c => c.IsAvailable));
                                    menus.AddRange(permission.Menu.Where(c => c.IsAvailable));
                                }
                            });
                        }
                    }
                    else
                    {
                        //角色不可用，取差集
                        ugp.Role.Permission.ForEach(p => controls = controls.Except(p.Controls).Where(c => c.IsAvailable).ToList());
                        ugp.Role.Permission.ForEach(p => menus = menus.Except(p.Menu).Where(c => c.IsAvailable).ToList());
                    }
                });
            }
            return (apps, users, groups, roles.Distinct().ToList(), permissions.Distinct().ToList(), controls.Distinct().ToList(), menus.Distinct().ToList());
        }

    }
}