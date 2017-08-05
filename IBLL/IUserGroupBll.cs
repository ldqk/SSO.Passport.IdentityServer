using System.Collections.Generic;
using Models.Entity;

namespace IBLL
{
    public partial interface IUserGroupBll
    {
        /// <summary>
        /// 根据名称找权限
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        UserGroup GetGroupByName(string name);

        /// <summary>
        /// 判断权限名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GroupNameExist(string name);

        /// <summary>
        /// 根据权限名找所属的用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEnumerable<UserInfo> GetUserInfoList(string name);
    }
}