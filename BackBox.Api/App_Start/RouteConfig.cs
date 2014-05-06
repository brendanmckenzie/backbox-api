using System.Web.Mvc;
using System.Web.Routing;

namespace BackBox.Api
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Connect", "connect", new { controller = "Api", action = "Connect" });
            routes.MapRoute("Set Name", "set-name/{name}", new { controller = "Api", action = "SetName" });
            routes.MapRoute("Set Bounds", "set-bounds/{lat}/{lng}/{radius}", new { controller = "Api", action = "SetBounds" });
            routes.MapRoute("Send", "send", new { controller = "Api", action = "Send" });
            routes.MapRoute("Get Latest", "get-latest", new { controller = "Api", action = "GetLatest" });
        }
    }
}
