using Sitecore.Pipelines;
using System.Web.Routing;

namespace GrandfathersApiary.Feature.Search.Infrastructure.Pipelines
{
    public class InitializeRoutes
    {
        public void Process(PipelineArgs args)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}