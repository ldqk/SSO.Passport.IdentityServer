using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Masuit.Tools.Logging;
using Z.BulkOperations;

namespace SSO.Passport.IdentityServer
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            LicenseManager.AddLicense("83;100-3EQWD", "96389A270C0805BB1B8D190BC2E3685D");
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            StartupConfig.Startup();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string httpMethod = Request.HttpMethod;
            if (httpMethod.Equals("OPTIONS", StringComparison.InvariantCultureIgnoreCase) || httpMethod.Equals("HEAD", StringComparison.InvariantCultureIgnoreCase))
            {
                Response.End();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpException exception = ((HttpApplication)sender).Context.Error as HttpException;
            int? errorCode = exception?.GetHttpCode() ?? 503;
            switch (errorCode)
            {
                case 404:
                    Response.Redirect("/error");
                    break;
                case 503:
                    LogManager.Error(exception);
                    Response.Redirect("/ServiceUnavailable");
                    break;
                default:
                    return;
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}