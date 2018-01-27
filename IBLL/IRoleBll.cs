using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Models.Dto;
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
        /// <summary>
        /// 通过存储过程获得自己以及自己所有的子元素集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DbRawSqlQuery<RoleOutputDto> GetSelfAndChildrenByParentId(int id);

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<int> GetParentIdById(int id);

        /// <summary>
        /// 获取角色所有的访问控制详情
        /// </summary>
        /// <returns></returns>
        (IQueryable<ClientApp>, IQueryable<UserInfo>, IQueryable<UserGroupRole>, List<Role>, List<Permission>, List<Control>, List<Menu>) Details(Role role);
    }
}