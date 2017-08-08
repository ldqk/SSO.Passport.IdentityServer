using System;
using System.Collections.Generic;
using Models.Dto;
using Models.Entity;
using Models.Enum;

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
        UserInfo Login(string username, string password);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        UserInfo Register(UserInfo userInfo);

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
        /// <param name="user"></param>
        /// <returns></returns>
        IList<Function> GetPermissionList(UserInfo user);

        /// <summary>
        /// 根据类型获取权限列表
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<Function> GetPermissionList(UserInfo userInfo, FunctionType type);

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

    }
}