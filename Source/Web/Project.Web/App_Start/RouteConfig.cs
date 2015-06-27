namespace Project.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "FileManager", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Project.Web.Controllers" }
            );
        }
    }
}
