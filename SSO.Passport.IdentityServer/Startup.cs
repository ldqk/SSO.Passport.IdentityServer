using System.Configuration;
using System.Web;
using Hangfire;
using Hangfire.Dashboard;
using Masuit.Tools.Net;
using Masuit.Tools.NoSQL;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Models.Dto;
using Owin;
using SSO.Passport.IdentityServer;

[assembly: OwinStartup(typeof(Startup))]

namespace SSO.Passport.IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //配置任务持久化到内存
            //GlobalConfiguration.Configuration.UseMemoryStorage();
            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString);

            //启用dashboard
            app.UseHangfireServer(new BackgroundJobServerOptions { WorkerCount = 10 });
            app.UseHangfireDashboard("/taskcenter", new DashboardOptions()
            {
                Authorization = new[] { new MyRestrictiveAuthorizationFilter() }
            }); //注册dashboard的路由地址
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    public class MyRestrictiveAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public RedisHelper RedisHelper { get; set; } = new RedisHelper();
        public bool Authorize(DashboardContext context)
        {
#if DEBUG
            return true;
#endif
            UserInfoOutputDto user = HttpContext.Current.Session.GetByCookieRedis<UserInfoOutputDto>() ?? new UserInfoOutputDto();
            return user.IsMaster;
        }
    }
}
