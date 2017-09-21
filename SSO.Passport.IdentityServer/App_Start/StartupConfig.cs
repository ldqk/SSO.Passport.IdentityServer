using System;
using System.Web.Mvc;
using Common;
using FluentScheduler;
using Masuit.Tools.Logging;

namespace SSO.Passport.IdentityServer
{
    public class StartupConfig
    {
        public static void Startup()
        {
            ViewEngines.Engines.RemoveAt(0);
            LogManager.LogDirectory = LogManager.LogDirectory = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Logs\"; //设置日志目录
            AutofacConfig.Register();
            RegisterAutomapper.Excute();
            Registry reg = new Registry();
            reg.Schedule(() => CollectRunningInfo.Start()).ToRunNow().AndEvery(2).Seconds();
            JobManager.Initialize(reg); //初始化定时器
        }
    }
}