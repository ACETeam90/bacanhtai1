using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(S4T_HaTinh.Startup))]
namespace S4T_HaTinh
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
