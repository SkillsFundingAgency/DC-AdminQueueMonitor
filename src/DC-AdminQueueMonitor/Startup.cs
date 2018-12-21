using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DC_AdminQueueMonitor.Startup))]
namespace DC_AdminQueueMonitor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
