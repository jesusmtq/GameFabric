using System;
using System.Web.Http;
using Owin;
using Microsoft.Owin.Host.HttpListener;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;
using GameFabric.SignalR;

namespace Services.Gateway
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            //HttpConfiguration config = new HttpConfiguration();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //appBuilder.UseWebApi(config);

            //Connection setup
            OwinHttpListener listener = (OwinHttpListener)appBuilder.Properties[typeof(OwinHttpListener).FullName];
            int maxAccepts = 0;
            int maxRequests = 0;
            listener.GetRequestProcessingLimits(out maxAccepts, out maxRequests);
            listener.SetRequestProcessingLimits(maxAccepts * 8, maxRequests);
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            //config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHanlder());
            config.MapHttpAttributeRoutes();
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
            FormatterConfig.ConfigureFormatters(config.Formatters);
            RouteConfig.RegisterRoutes(config.Routes);
            appBuilder.UseWebApi(config);

            //Then do SignalR
            //GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
            appBuilder.MapSignalR();
            var fabricProtData = new Lazy<FabricProtectedData>(() => new FabricProtectedData());
            GlobalHost.DependencyResolver.Register(typeof(Microsoft.AspNet.SignalR.Infrastructure.IProtectedData), () => fabricProtData.Value);
            //And init
            config.EnsureInitialized();
            //ConnectionHandler.Instance.MapContext(GlobalHost.ConnectionManager.GetHubContext<InterfaceHub>().Clients);


        }
    }
    public static class FormatterConfig
    {
        public static void ConfigureFormatters(MediaTypeFormatterCollection formatters)
        {
            JsonSerializerSettings settings = formatters.JsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.None; //Formatting.Indented;
        }
    }

    public static class RouteConfig
    {
        /// <summary>
        ///     Routing registration.
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
