using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Security;
using Masuit.Tools.Win32;
using Models.Application;
using Models.Dto;
using Models.Entity;

namespace BLL
{
    public partial class UserInfoBll
    {
        /// <summary>
        /// 根据用户名获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UserInfo GetByUsername(string name)
        {
            return GetFirstEntity(u => u.Username.Equals(name));
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserInfoDto Login(string username, string password)
        {
            UserInfo userInfo = GetByUsername(username);
            if (userInfo != null)
            {
                string key = userInfo.SaltKey;
                string pwd = userInfo.Password;
                password = password.MDString2(key);
                if (pwd.Equals(password))
                {
                    return userInfo.Mapper<UserInfoDto>();
                }
            }
            return null;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public UserInfoDto Register(UserInfo userInfo)
        {
            UserInfo exist = GetFirstEntity(u => u.Username.Equals(userInfo.Username) || u.Email.Equals(userInfo.Email) || u.PhoneNumber.Equals(userInfo.PhoneNumber));
            if (exist is null)
            {
                userInfo.Id = Guid.NewGuid();
                var salt = $"{new Random().StrictNext()}{DateTime.Now.GetTotalMilliseconds()}".MDString2(Guid.NewGuid().ToString()).Base64Encrypt();
                userInfo.Password = userInfo.Password.MDString2(salt);
                userInfo.SaltKey = salt;
                UserInfo added = AddEntity(userInfo);
                int line = SaveChanges();
                return line > 0 ? added.Mapper<UserInfoDto>() : null;
            }
            return null;
        }

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UsernameExist(string name)
        {
            UserInfo userInfo = GetByUsername(name);
            return userInfo != null;
        }

        /// <summary>
        /// 检查邮箱是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool EmailExist(string email) => GetFirstEntityNoTracking(u => u.Email.Equals(email)) != null;

        /// <summary>
        /// 检查电话号码是否存在
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool PhoneExist(string num) => GetFirstEntityNoTracking(u => u.PhoneNumber.Equals(num)) != null;

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<Control> GetPermissionList(Guid id)
        {
            var user = GetFirstEntity(u => u.Id.Equals(id));
            List<Control> list = new List<Control>(); //所有允许的权限
            if (user != null)
            {
                //1.0 用户-角色-权限-功能 主线，权限的优先级最低
                user.Role.ForEach(r => r.Permission.ForEach(p => list.AddRange(p.Controls.Where(c => c.IsAvailable))));

                //2.0 用户-用户组-角色-权限，权限的优先级其次
                user.UserGroup?.ForEach(g => g.UserGroupPermission.ForEach(ugp =>
                {
                    if (ugp.HasRole)
                    {
                        ugp.Role.Permission.ForEach(p => list.AddRange(p.Controls.Where(c => c.IsAvailable)));
                    }
                    else
                    {
                        ugp.Role.Permission.ForEach(p => list = list.Except(p.Controls).ToList());
                    }
                }));

                //3.0 用户-权限-功能 临时权限，权限的优先级最高
                user.UserPermission?.ForEach(p =>
                {
                    if (p.HasPermission)
                    {
                        list.AddRange(p.Permission.Controls.Where(c => c.IsAvailable));
                    }
                    else
                    {
                        list = list.Except(p.Permission.Controls).ToList();
                    }
                });
            }
            return list.Where(c => c.IsAvailable).Distinct(new AccessControlComparision()).ToList();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="name">用户名，邮箱或者电话号码</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        public bool ChangePassword(string name, string oldPwd, string newPwd)
        {
            UserInfo userInfo = GetByUsername(name);
            if (userInfo != null)
            {
                string key = userInfo.SaltKey;
                string pwd = userInfo.Password;
                oldPwd = oldPwd.MDString2(key);
                if (pwd.Equals(oldPwd))
                {
                    var salt = $"{new Random().StrictNext()}{DateTime.Now.GetTotalMilliseconds()}".MDString2(Guid.NewGuid().ToString()).Base64Encrypt();
                    userInfo.Password = newPwd.MDString2(salt);
                    userInfo.SaltKey = salt;
                    return SaveChanges() > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        public bool ChangePassword(Guid id, string oldPwd, string newPwd)
        {
            UserInfo userInfo = GetById(id);
            if (userInfo != null)
            {
                string key = userInfo.SaltKey;
                string pwd = userInfo.Password;
                oldPwd = oldPwd.MDString2(key);
                if (pwd.Equals(oldPwd))
                {
                    var salt = $"{new Random().StrictNext()}{DateTime.Now.GetTotalMilliseconds()}".MDString2(Guid.NewGuid().ToString()).Base64Encrypt();
                    userInfo.Password = newPwd.MDString2(salt);
                    userInfo.SaltKey = salt;
                    return SaveChanges() > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        public bool ResetPassword(string name, string newPwd = "123456")
        {
            UserInfo userInfo = GetByUsername(name);
            if (userInfo != null)
            {
                var salt = $"{new Random().StrictNext()}{DateTime.Now.GetTotalMilliseconds()}".MDString2(Guid.NewGuid().ToString()).Base64Encrypt();
                userInfo.Password = newPwd.MDString2(salt);
                userInfo.SaltKey = salt;
                return SaveChanges() > 0;
            }
            return false;
        }

        /// <summary>
        /// 获取操作权限
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ControlOutputDto> GetAccessControls(string appid, Guid id)
        {
            DataContext context = BaseDal.GetDataContext();
            ClientApp app = context.ClientApp.FirstOrDefault(a => a.AppId.Equals(appid));//获取客户端子系统应用
            UserInfo user = GetById(id);//获取用户
            List<Control> list = new List<Control>();
            if (app != null && user != null)
            {
                //1.0 用户-角色-权限-功能 主线，权限的优先级最低
                user.Role.ForEach(r =>
                {
                    int[] rids = context.Database.SqlQuery<int>("exec sp_getParentRoleIdByChildId " + r.Id).ToArray();//拿到所有上级角色
                    foreach (int i in rids)
                    {
                        Role role = context.Role.FirstOrDefault(o => o.Id == i);
                        role?.Permission.ForEach(p =>
                        {
                            int[] pids = context.Database.SqlQuery<int>("exec sp_getParentPermissionIdByChildId " + p.Id).ToArray();//拿到所有上级权限
                            foreach (int s in pids)
                            {
                                Permission permission = context.Permission.FirstOrDefault(x => x.Id == s);
                                list.AddRange(permission.Controls.Where(c => c.IsAvailable));
                            }
                        });
                    }
                });

                //2.0 用户-用户组-角色-权限，权限的优先级其次
                user.UserGroup.ForEach(g =>
                {
                    //2.1 拿到所有上级用户组
                    int[] gids = context.Database.SqlQuery<int>("exec sp_getParentGroupIdByChildId " + g.Id).ToArray();//拿到所有上级用户组
                    foreach (int i in gids)
                    {
                        UserGroup group = context.UserGroup.FirstOrDefault(u => u.Id == i);
                        List<int> noRoleIds = @group.UserGroupPermission.Where(x => !x.HasRole).Select(x => x.Id).ToList();//没有角色的id集合
                        group?.UserGroupPermission.ForEach(ugp =>
                        {
                            if (ugp.HasRole)
                            {
                                //角色可用，取并集
                                //2.2 拿到所有上级角色，并排除掉角色不可用的角色id
                                int[] rids = context.Database.SqlQuery<int>("exec sp_getParentRoleIdByChildId " + ugp.Role.Id).Except(noRoleIds).ToArray();//拿到所有上级角色，并排除掉角色不可用的角色id
                                foreach (int r in rids)
                                {
                                    Role role = context.Role.FirstOrDefault(o => o.Id == r);
                                    role?.Permission.ForEach(p =>
                                    {
                                        //2.3 拿到所有上级权限
                                        int[] pids = context.Database.SqlQuery<int>("exec sp_getParentPermissionIdByChildId " + p.Id).ToArray();//拿到所有上级权限
                                        foreach (int s in pids)
                                        {
                                            Permission permission = context.Permission.FirstOrDefault(x => x.Id == s);
                                            list.AddRange(permission.Controls.Where(c => c.IsAvailable));
                                        }
                                    });
                                }
                            }
                            else
                            {
                                //角色不可用，取差集
                                ugp.Role.Permission.ForEach(p => list = list.Except(p.Controls).Where(c => c.IsAvailable).ToList());
                            }
                        });
                    }
                });

                //3.0 用户-权限-功能 临时权限，权限的优先级最高
                List<int> noPermissionIds = user.UserPermission.Where(p => !p.HasPermission).Select(p => p.Id).ToList();//没有权限的id集合
                user.UserPermission?.ForEach(p =>
                {
                    if (p.HasPermission)
                    {
                        //临时权限可用，取并集
                        //3.1 拿到所有上级权限，并排除掉没有权限的角色id
                        int[] pids = context.Database.SqlQuery<int>("exec sp_getParentPermissionIdByChildId " + p.Id).Except(noPermissionIds).ToArray();//拿到所有上级权限，并排除掉没有权限的角色id
                        foreach (int i in pids)
                        {
                            Permission permission = context.Permission.FirstOrDefault(x => x.Id == i);
                            list.AddRange(permission.Controls.Where(c => c.IsAvailable));
                        }
                    }
                    else
                    {
                        //临时权限不可用，取差集
                        list = list.Except(p.Permission.Controls.Where(c => c.IsAvailable)).ToList();
                    }
                });
                return list.Where(c => c.IsAvailable && c.ClientAppId == app.Id).Distinct(new AccessControlComparision()).ToList().Mapper<List<ControlOutputDto>>();
            }
            return new List<ControlOutputDto>();
        }

        /// <summary>
        /// 获取菜单权限
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<MenuOutputDto> GetMenus(string appid, Guid id)
        {
            DataContext context = BaseDal.GetDataContext();
            ClientApp app = context.ClientApp.FirstOrDefault(a => a.AppId.Equals(appid));//获取客户端子系统应用
            UserInfo user = GetById(id);//获取用户
            List<Menu> list = new List<Menu>();
            if (app != null && user != null)
            {
                //1.0 用户-角色-权限-功能 主线，权限的优先级最低
                user.Role.ForEach(r =>
                {
                    int[] rids = context.Database.SqlQuery<int>("exec sp_getParentRoleIdByChildId " + r.Id).ToArray();//拿到所有上级角色
                    foreach (int i in rids)
                    {
                        Role role = context.Role.FirstOrDefault(o => o.Id == i);
                        role?.Permission.ForEach(p =>
                        {
                            int[] pids = context.Database.SqlQuery<int>("exec sp_getParentPermissionIdByChildId " + p.Id).ToArray();//拿到所有上级权限
                            foreach (int s in pids)
                            {
                                Permission permission = context.Permission.FirstOrDefault(x => x.Id == s);
                                list.AddRange(permission.Menu.Where(c => c.IsAvailable));
                            }
                        });
                    }
                });

                //2.0 用户-用户组-角色-权限，权限的优先级其次
                user.UserGroup.ForEach(g =>
                {
                    //2.1 拿到所有上级用户组
                    int[] gids = context.Database.SqlQuery<int>("exec sp_getParentGroupIdByChildId " + g.Id).ToArray();//拿到所有上级用户组
                    foreach (int i in gids)
                    {
                        UserGroup group = context.UserGroup.FirstOrDefault(u => u.Id == i);
                        List<int> noRoleIds = @group.UserGroupPermission.Where(x => !x.HasRole).Select(x => x.Id).ToList();//没有角色的id集合
                        group?.UserGroupPermission.ForEach(ugp =>
                        {
                            if (ugp.HasRole)
                            {
                                //角色可用，取并集
                                //2.2 拿到所有上级角色，并排除掉角色不可用的角色id
                                int[] rids = context.Database.SqlQuery<int>("exec sp_getParentRoleIdByChildId " + ugp.Role.Id).Except(noRoleIds).ToArray();//拿到所有上级角色，并排除掉角色不可用的角色id
                                foreach (int r in rids)
                                {
                                    Role role = context.Role.FirstOrDefault(o => o.Id == r);
                                    role?.Permission.ForEach(p =>
                                    {
                                        //2.3 拿到所有上级权限
                                        int[] pids = context.Database.SqlQuery<int>("exec sp_getParentPermissionIdByChildId " + p.Id).ToArray();//拿到所有上级权限
                                        foreach (int s in pids)
                                        {
                                            Permission permission = context.Permission.FirstOrDefault(x => x.Id == s);
                                            list.AddRange(permission.Menu.Where(c => c.IsAvailable));
                                        }
                                    });
                                }
                            }
                            else
                            {
                                //角色不可用，取差集
                                ugp.Role.Permission.ForEach(p => list = list.Except(p.Menu).Where(c => c.IsAvailable).ToList());
                            }
                        });
                    }
                });

                //3.0 用户-权限-功能 临时权限，权限的优先级最高
                List<int> noPermissionIds = user.UserPermission.Where(p => !p.HasPermission).Select(p => p.Id).ToList();//没有权限的id集合
                user.UserPermission?.ForEach(p =>
                {
                    if (p.HasPermission)
                    {
                        //临时权限可用，取并集
                        //3.1 拿到所有上级权限，并排除掉没有权限的角色id
                        int[] pids = context.Database.SqlQuery<int>("exec sp_getParentPermissionIdByChildId " + p.Id).Except(noPermissionIds).ToArray();//拿到所有上级权限，并排除掉没有权限的角色id
                        foreach (int i in pids)
                        {
                            Permission permission = context.Permission.FirstOrDefault(x => x.Id == i);
                            list.AddRange(permission.Menu.Where(c => c.IsAvailable));
                        }
                    }
                    else
                    {
                        //临时权限不可用，取差集
                        list = list.Except(p.Permission.Menu.Where(c => c.IsAvailable)).ToList();
                    }
                });
                return list.Where(c => c.IsAvailable && c.ClientAppId == app.Id).Distinct(new MenuComparision()).ToList().Mapper<List<MenuOutputDto>>();
            }
            return new List<MenuOutputDto>();
        }
    }

    public class AccessControlComparision : IEqualityComparer<Control>
    {
        public bool Equals(Control x, Control y)
        {
            return x.ClientAppId.Equals(y.ClientAppId) && x.Controller.Equals(y.Controller) && x.Action.Equals(y.Action);
        }

        public int GetHashCode(Control obj)
        {
            return 0;
        }
    }
    public class MenuComparision : IEqualityComparer<Menu>
    {
        public bool Equals(Menu x, Menu y)
        {
            return x.ClientAppId.Equals(y.ClientAppId) && x.Name.Equals(y.Name);
        }

        public int GetHashCode(Menu obj)
        {
            return 0;
        }
    }
}