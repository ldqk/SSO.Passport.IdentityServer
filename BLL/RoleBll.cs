using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Masuit.Tools.Net;
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
            return WebExtension.GetDbContext<DataContext>().Database.SqlQuery<RoleOutputDto>("exec sp_getChildrenRoleByParentId " + id);
        }

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetParentIdById(int id)
        {
            DbRawSqlQuery<int> raw = WebExtension.GetDbContext<DataContext>().Database.SqlQuery<int>("exec sp_getParentRoleIdByChildId " + id);
            if (raw.Any())
            {
                return raw.FirstOrDefault();
            }
            return 0;
        }
    }
}