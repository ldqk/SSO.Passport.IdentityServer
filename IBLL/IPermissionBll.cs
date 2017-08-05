using System.Collections.Generic;
using Models.Entity;

namespace IBLL
{
    public partial interface IPermissionBll
    {
        /// <summary>
        /// 根据名称找权限
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Permission GetPermissionByName(string name);

        /// <summary>
        /// 判断权限名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool PermissionNameExist(string name);

        /// <summary>
        /// 根据权限名找所属的用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerable<UserInfo> GetUserInfoList(string name);

        /// <summary>
        /// 根据权限名找所属的角色列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerable<Role> GetRoleList(string name);
    }
}