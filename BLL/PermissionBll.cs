using System.Collections.Generic;
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
            return GetPermissionByName(name) is null;
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
    }
}