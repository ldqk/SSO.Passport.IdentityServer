using System.Collections.Generic;
using Models.Entity;

namespace IBLL
{
    public partial interface IRoleBll
    {
        /// <summary>
        /// 根据名称找角色
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Role GetRoleByName(string name);

        /// <summary>
        /// 判断角色名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool RoleNameExist(string name);

        /// <summary>
        /// 根据角色名找所属的用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerable<UserInfo> GetUserInfoList(string name);

        /// <summary>
        /// 根据角色名找所属的用户组
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerable<UserGroup> GetUserGroupList(string name);
    }
}