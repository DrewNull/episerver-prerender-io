namespace PrerenderForEpiserver
{
    using System.Web;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;

    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class PrerenderInitialization : IInitializableHttpModule
    {
        private readonly PrerenderHandler _prerenderHandler = new PrerenderHandler();

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void InitializeHttpEvents(HttpApplication application)
        {
            application.BeginRequest += (sender, e) => _prerenderHandler.DoPrerender(sender as HttpApplication);
        }
    }
}
