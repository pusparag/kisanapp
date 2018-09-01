using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { area = "HelpPage", controller = "Help", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                new[] { "WebApplication1.Areas.HelpPage.Controllers" }
            ).DataTokens.Add("area", "HelpPage");
        }
    }
}
