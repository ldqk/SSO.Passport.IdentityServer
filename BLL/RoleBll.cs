using System.Collections.Generic;
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
            ICollection<UserGroupPermission> ps = GetRoleByName(name).UserGroupPermission;
            foreach (UserGroupPermission g in ps)
            {
                yield return g.UserGroup;
            }
        }
    }
}