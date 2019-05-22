using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "host",
                url: "",
                defaults: new { controller = "Home", action = "Catalog" }
            );

            routes.MapRoute(
                name: "localhost",
                url: "localhost",
                defaults: new { controller = "Home", action = "Catalog" }
            );

            routes.MapRoute(
                name: "catalog",
                url: "Catalog/",
                defaults: new { controller = "Home", action = "Catalog" }
            );
            
            routes.MapRoute(
                name: "starter",
                url: "Home/Catalog",
                defaults: new { controller = "Home", action = "Catalog" }
            );

            routes.MapRoute(
                name: "about",
                url: "About",
                defaults: new { controller = "Home", action = "About" }
            );

            routes.MapRoute(
                name: "brand",
                url: "Catalog/{brandName}",
                defaults: new { controller = "Home", action = "Brand" }
            );

            routes.MapRoute(
                name: "model",
                url: "Catalog/{brandName}/{model}",
                defaults: new { controller = "Home", action = "ModelInfo" }
            );

            // traps all other urls as 404
            routes.MapRoute(
                "CatchAll",
                "{*path}",
                new { Controller = "Home", Action = "NotFound" }
            );
        }
    }
}