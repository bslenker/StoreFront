using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StoreFront.UI.MVC.Startup))]
namespace StoreFront.UI.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
