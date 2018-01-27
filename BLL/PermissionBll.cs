using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Masuit.Tools;
using Models.Application;
using Models.Dto;
using Models.Entity;

namespace BLL
{
    public partial class PermissionBll
    {
        /// <summary>
        /// 根据名称找权限
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Permission GetPermissionByName(string name)
        {
            return GetFirstEntity(p => p.PermissionName.Equals(name));
        }

        /// <summary>
        /// 判断权限名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool PermissionNameExist(string name)
        {
            return GetPermissionByName(name) != null;
        }

        /// <summary>
        /// 根据权限名找所属的用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<UserInfo> GetUserInfoList(string name)
        {
            ICollection<UserPermission> ps = GetPermissionByName(name).UserPermission;
            foreach (UserPermission p in ps)
            {
                yield return p.UserInfo;
            }
        }

        /// <summary>
        /// 根据权限名找所属的角色列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Role> GetRoleList(string name)
        {
            return GetPermissionByName(name).Role;
        }
        /// <summary>
        /// 通过存储过程获得自己以及自己所有的子元素集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DbRawSqlQuery<PermissionOutputDto> GetSelfAndChildrenByParentId(int id)
        {
            return BaseDal.GetDataContext().Database.SqlQuery<PermissionOutputDto>("exec sp_getChildrenPermissionByParentId " + id);
        }

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<int> GetParentIdById(int id)
        {
            return BaseDal.GetDataContext().Database.SqlQuery<int>("exec sp_getParentPermissionIdByChildId " + id).ToList();
        }


        /// <summary>
        /// 获取权限所有的访问控制详情，包括父级继承
        /// </summary>
        /// <returns></returns>
        public (IQueryable<ClientApp>, IQueryable<UserPermission>, List<Role>, List<Permission>, List<Control>, List<Menu>) Details(Permission permission)
        {
            DataContext context = BaseDal.GetDataContext();
            IQueryable<ClientApp> apps = new ClientAppBll().LoadEntities(a => a.Permissions.Any(p => p.Id == permission.Id));//permission.ClientApp.AsQueryable();
            IQueryable<UserPermission> users = new UserPermissionBll().LoadEntities(a => a.PermissionId == permission.Id);
            List<Role> roles = new List<Role>();
            List<Control> controls = new List<Control>();
            List<Menu> menus = new List<Menu>();
            List<Permission> permissions = new List<Permission>();

            var pids = GetParentIdById(permission.Id); //拿到所有上级权限
            foreach (int id in pids)
            {
                Permission p = context.Permission.FirstOrDefault(x => x.Id == id);
                if (id != permission.Id)
                {
                    permissions.Add(p);
                }
                controls.AddRange(p.Controls.Where(c => c.IsAvailable));
                menus.AddRange(p.Menu.Where(c => c.IsAvailable));
            }

            permission.Role.Distinct().ForEach(r =>
            {
                List<int> rids = context.Database.SqlQuery<int>("exec sp_getParentRoleIdByChildId " + r.Id).ToList();
                List<Role> list = context.Role.Where(role => rids.Contains(role.Id)).ToList();
                roles.AddRange(list);
            });
            return (apps, users, roles.Distinct().ToList(), permissions, controls.Distinct().ToList(), menus.Distinct().ToList());
        }
    }
}