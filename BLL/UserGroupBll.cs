using System.Collections.Generic;
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
    }
}