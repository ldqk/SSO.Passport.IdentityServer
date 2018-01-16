using System;
using System.Configuration;
using Common;
using Hangfire;
using Hangfire.Console;
using Masuit.Tools.Win32;
using Models.Application;
using SSO.Passport.IdentityServer.Models.Hangfire;

namespace SSO.Passport.IdentityServer
{
    public class HangfireConfig
    {
        public static void Register()
        {
            #region Hangfire配置

            //GlobalConfiguration.Configuration.UseMemoryStorage();
            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString).UseConsole();
            #region 实现类注册

            GlobalConfiguration.Configuration.UseAutofacActivator(AutofacConfig.Container);

            #endregion

            #region 服务启动

            Server = new BackgroundJobServer(new BackgroundJobServerOptions
            {
                ServerName = $"{Environment.MachineName}", //服务器名称
                SchedulePollingInterval = TimeSpan.FromSeconds(1),
                ServerCheckInterval = TimeSpan.FromSeconds(1),
                WorkerCount = Environment.ProcessorCount * 2,
                //Queues = new[] { "masuit" } //队列名
            });
            #endregion

            #endregion
            RecurringJob.AddOrUpdate(() => Windows.ClearMemorySilent(), Cron.Hourly);
            RecurringJob.AddOrUpdate(() => EverydayJob(), Cron.Daily, TimeZoneInfo.Local);
        }

        public static BackgroundJobServer Server { get; set; }
        
        public static void EverydayJob()
        {
            DataContext db = new DataContext();
            db.Database.ExecuteSqlCommand($@"DELETE FROM [HangFire].[Job] WHERE StateName='Succeeded' or StateName='Deleted';
                                                              UPDATE [HangFire].[AggregatedCounter] SET [Value] = (select count(1) from [HangFire].[Job] WHERE StateName<>'Succeeded' and StateName<>'Deleted')-1 WHERE [Key] = 'stats:succeeded'; 
                                                              UPDATE [HangFire].[AggregatedCounter] SET [Value] = 0 WHERE [Key] = 'stats:deleted'");
        }
        
    }
}