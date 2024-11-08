﻿using System.Web.Http;

namespace OnlineForms.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                //routeTemplate: "/api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}