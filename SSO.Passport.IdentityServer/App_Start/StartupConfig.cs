using System;
using System.IO;
using Common;
using Masuit.Tools.Logging;

namespace SSO.Passport.IdentityServer
{
    public class StartupConfig
    {
        public static void Startup()
        {
            LogManager.LogDirectory = LogManager.LogDirectory = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Logs\"; //设置日志目录
            AutofacConfig.Register();
            RegisterAutomapper.Excute();
        }
    }
}