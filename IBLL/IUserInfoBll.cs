using System;
using System.Collections.Generic;
using Models.Dto;
using Models.Entity;

namespace IBLL
{
    public partial interface IUserInfoBll
    {
        /// <summary>
        /// 根据用户名获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        UserInfo GetByUsername(string name);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserInfoDto Login(string username, string password);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        UserInfoDto Register(UserInfo userInfo);

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool UsernameExist(string name);

        /// <summary>
        /// 检查邮箱是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool EmailExist(string email);

        /// <summary>
        /// 检查电话号码是否存在
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        bool PhoneExist(string num);

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IList<Control> GetPermissionList(Guid id);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="name">用户名，邮箱或者电话号码</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        bool ChangePassword(string name, string oldPwd, string newPwd);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        bool ChangePassword(Guid id, string oldPwd, string newPwd);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        bool ResetPassword(string name, string newPwd = "123456");

        /// <summary>
        /// 获取操作权限
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        List<ControlOutputDto> GetAccessControls(string appid, Guid id);

        /// <summary>
        /// 获取菜单权限
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        List<MenuOutputDto> GetMenus(string appid, Guid id);

        /// <summary>
        /// 获取用户所拥有菜单和功能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        (List<MenuOutputDto>, List<ControlOutputDto>) GetAllAccess(Guid id);
    }
}