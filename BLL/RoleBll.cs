using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Masuit.Tools;
using Models.Application;
using Models.Dto;
using Models.Entity;

namespace BLL
{
    public partial class RoleBll
    {
        /// <summary>
        /// 根据名称找角色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Role GetRoleByName(string name)
        {
            return GetFirstEntity(r => r.RoleName.Equals(name));
        }

        /// <summary>
        /// 判断角色名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RoleNameExist(string name)
        {
            return GetRoleByName(name) != null;
        }

        /// <summary>
        /// 根据角色名找所属的用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<UserInfo> GetUserInfoList(string name)
        {
            return GetRoleByName(name).UserInfo;
        }

        /// <summary>
        /// 根据角色名找所属的用户组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<UserGroup> GetUserGroupList(string name)
        {
            ICollection<UserGroupRole> ps = GetRoleByName(name).UserGroupPermission;
            foreach (UserGroupRole g in ps)
            {
                yield return g.UserGroup;
            }
        }
        /// <summary>
        /// 通过存储过程获得自己以及自己所有的子元素集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DbRawSqlQuery<RoleOutputDto> GetSelfAndChildrenByParentId(int id)
        {
            return BaseDal.GetDataContext().Database.SqlQuery<RoleOutputDto>("exec sp_getChildrenRoleByParentId " + id);
        }

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<int> GetParentIdById(int id)
        {
            return BaseDal.GetDataContext().Database.SqlQuery<int>("exec sp_getParentRoleIdByChildId " + id).ToList();
        }

        /// <summary>
        /// 获取角色所有的访问控制详情
        /// </summary>
        /// <returns></returns>
        public (IQueryable<ClientApp>, IQueryable<UserInfo>, IQueryable<UserGroupRole>, List<Role>, List<Permission>, List<Control>, List<Menu>) Details(Role role)
        {
            DataContext context = BaseDal.GetDataContext();
            IQueryable<ClientApp> apps = new ClientAppBll().LoadEntities(a => a.Roles.Any(r => r.Id == role.Id));
            IQueryable<UserInfo> users = new UserInfoBll().LoadEntities(u => u.Role.Any(r => r.Id == role.Id));
            IQueryable<UserGroupRole> groups = new UserGroupRoleBll().LoadEntities(g => g.RoleId == role.Id);
            List<Control> controls = new List<Control>();
            List<Menu> menus = new List<Menu>();
            List<Permission> permissions = new List<Permission>();
            List<Role> roles = new List<Role>();

            var rids = GetParentIdById(role.Id); //拿到所有上级角色，并排除掉角色不可用的角色id
            foreach (int rid in rids)
            {
                Role r = context.Role.FirstOrDefault(o => o.Id == rid);
                if (r?.Id != role.Id)
                {
                    roles.Add(r);
                }
                r?.Permission.ForEach(p =>
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
            return (apps, users, groups, roles, permissions.Distinct().ToList(), controls.Distinct().ToList(), menus.Distinct().ToList());
        }

    }
}