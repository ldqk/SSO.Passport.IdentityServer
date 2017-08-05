using System;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Security;
using Masuit.Tools.Win32;
using Models.Entity;

namespace BLL
{
    public partial class UserInfoBll
    {
        public UserInfo GetByUsername(string name)
        {
            return GetFirstEntity(u => u.Username.Equals(name));
        }

        public UserInfo Login(string username, string password)
        {
            UserInfo userInfo = GetByUsername(username);
            if (userInfo != null)
            {
                var lastLoginTime = userInfo.LastLoginTime;
                userInfo.LastLoginTime = DateTime.Now;
                SaveChangesAsync();
                string key = userInfo.SaltKey;
                string pwd = userInfo.Password;
                password = password.MDString2(key);
                if (pwd.Equals(password))
                {
                    userInfo.LastLoginTime = lastLoginTime;
                    return userInfo;
                }
            }
            return null;
        }

        public UserInfo Register(UserInfo userInfo)
        {
            var salt = $"{new Random().StrictNext()}{DateTime.Now.GetTotalMilliseconds()}".MDString2(DateTime.Now.GetTotalMilliseconds().ToString()).Base64Encrypt();
            userInfo.Password = userInfo.Password.MDString2(salt);
            userInfo.SaltKey = salt;
            userInfo.LastLoginTime = DateTime.Now;
            UserInfo added = AddEntity(userInfo);
            int line = SaveChanges();
            return line > 0 ? added : null;
        }
    }
}