using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using Models.Dto;
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
        /// <summary>
        /// 通过存储过程获得自己以及自己所有的子元素集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DbRawSqlQuery<PermissionOutputDto> GetSelfAndChildrenByParentId(int id);

        /// <summary>
        /// 根据无级子级找顶级父级评论id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<int> GetParentIdById(int id);

        /// <summary>
        /// 获取权限所有的访问控制详情，包括父级继承
        /// </summary>
        /// <returns></returns>
        (List<ClientApp>, List<UserPermission>, List<Role>, List<Permission>, List<Control>, List<Menu>) Details(Permission permission);
    }
}