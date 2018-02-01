using System.Web.Mvc;
using System.Web.Routing;

namespace GrandfathersApiary.Feature.Search
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("search-ajaxsearch", "api/feature/search/ajaxsearch", new { controller = "Search", action = "AjaxSearchResults", id = UrlParameter.Optional });
        }
    }
}