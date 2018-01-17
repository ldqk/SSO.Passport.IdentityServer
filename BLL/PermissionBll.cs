using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Masuit.Tools.Net;
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
            return WebExtension.GetDbContext<DataContext>().Database.SqlQuery<PermissionOutputDto>("exec sp_getChildrenPermissionByParentId " + id);
        }

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetParentIdById(int id)
        {
            DbRawSqlQuery<int> raw = WebExtension.GetDbContext<DataContext>().Database.SqlQuery<int>("exec sp_getParentPermissionIdByChildId " + id);
            if (raw.Any())
            {
                return raw.FirstOrDefault();
            }
            return 0;
        }
    }
}