using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;

namespace SSO.Passport.IdentityServer
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
#if DEBUG
            MiniProfilerEF6.Initialize();
#endif
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            StartupConfig.Startup();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
#if DEBUG
            MiniProfiler.Start();
#endif
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
#if DEBUG
            MiniProfiler.Stop();
#endif
        }

        protected void Application_Error(object sender, EventArgs e)
        {
#if DEBUG
            throw (Exception)sender;
#else
            Response.Redirect("/Error");
#endif
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}