using System;
using System.Dynamic;
using Masuit.Tools.NoSQL;

namespace SSO.Passport.IdentityServer.Models
{
    public static class SessionHelper
    {
        public static RedisHelper RedisHelper { get; set; } = new RedisHelper();

        public static void Set(string key, object value)
        {
            RedisHelper.SetString(key, value, TimeSpan.FromMinutes(20));
        }

        public static T Get<T>(string key)
        {
            RedisHelper.Expire(key, TimeSpan.FromMinutes(20));
            return RedisHelper.GetString<T>(key);
        }
    }
}