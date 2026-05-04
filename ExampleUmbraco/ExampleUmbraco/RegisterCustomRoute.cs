using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;

namespace ExampleUmbraco
{
    // This class auto run first when Umbraco run
    public class RegisterCustomRoute : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            RouteTable.Routes.MapRoute(
                name: "CustomAdminRoute",
                url: "admin/{action}/{id}",
                defaults: new
                {
                    controller = "Admin",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}