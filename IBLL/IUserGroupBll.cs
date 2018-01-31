using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Models.Dto;
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

        /// <summary>
        /// 通过存储过程获得自己以及自己所有的子元素集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DbRawSqlQuery<UserGroupOutputDto> GetSelfAndChildrenByParentId(int id);

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<int> GetParentIdById(int id);

        /// <summary>
        /// 获取用户组所有的访问控制详情
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        (IQueryable<ClientApp>, IQueryable<UserInfo>, List<UserGroup>, List<UserGroupRole>, List<Permission>, List<Control>, List<Menu>) Details(UserGroup @group);
    }
}