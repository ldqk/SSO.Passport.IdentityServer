using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SSO.Passport.IdentityServer.Startup))]

namespace SSO.Passport.IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
