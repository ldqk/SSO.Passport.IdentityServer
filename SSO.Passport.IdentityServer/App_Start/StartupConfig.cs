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
            //移除aspx视图引擎
            ViewEngines.Engines.RemoveAt(0);
            LogManager.LogDirectory = LogManager.LogDirectory = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Logs\"; //设置日志目录
            AutofacConfig.RegisterMVC();
            RegisterAutomapper.Excute();
            HangfireConfig.Register();

            Registry reg = new Registry();
            reg.Schedule(() => CollectRunningInfo.Start()).ToRunNow().AndEvery(5).Seconds();
            JobManager.Initialize(reg);//初始化定时器
        }

    }
}