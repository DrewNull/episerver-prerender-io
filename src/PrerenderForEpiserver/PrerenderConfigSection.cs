namespace PrerenderForEpiserver
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public sealed class PrerenderConfigSection : ConfigurationSection
    {
        public const string DefaultPrerenderServiceUrl = "http://service.prerender.io";

        [ConfigurationProperty("crawlerUserAgents")]
        public string CrawlerUserAgentsString
        {
            get => (string)this["crawlerUserAgents"];
            set => this["crawlerUserAgents"] = value;
        }

        [ConfigurationProperty("extensionsToIgnore")]
        public string ExtensionsToIgnoreString
        {
            get => (string)this["extensionsToIgnore"];
            set => this["extensionsToIgnore"] = value;
        }

        [ConfigurationProperty("naughtylist")]
        public string NaughtylistString
        {
            get => (string)this["naughtylist"];
            set => this["naughtylist"] = value;
        }

        [ConfigurationProperty("nicelist")]
        public string NicelistString
        {
            get => (string)this["nicelist"];
            set => this["nicelist"] = value;
        }

        [ConfigurationProperty("prerenderServiceUrl", DefaultValue = DefaultPrerenderServiceUrl)]
        public string PrerenderServiceUrl
        {
            get
            {
                var prerenderServiceUrl = (string)this["prerenderServiceUrl"];
                return string.IsNullOrWhiteSpace(prerenderServiceUrl) ? DefaultPrerenderServiceUrl : prerenderServiceUrl;
            }

            set => this["prerenderServiceUrl"] = value;
        }

        [ConfigurationProperty("Proxy")]
        public ProxyConfigElement Proxy
        {
            get => (ProxyConfigElement)this["Proxy"];
            set => this["Proxy"] = value;
        }

        [ConfigurationProperty("stripApplicationNameFromRequestUrl", DefaultValue = false)]
        public bool StripApplicationNameFromRequestUrl
        {
            get => (bool)this["stripApplicationNameFromRequestUrl"];
            set => this["stripApplicationNameFromRequestUrl"] = value;
        }

        [ConfigurationProperty("token")]
        public string Token
        {
            get => (string)this["token"];
            set => this["token"] = value;
        }

        public IEnumerable<string> CrawlerUserAgents
        {
            get => this.CrawlerUserAgentsString?.Trim()?.Split(',') ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> ExtensionsToIgnore
        {
            get => this.ExtensionsToIgnoreString?.Trim()?.Split(',') ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Nicelist
        {
            get => this.NicelistString?.Trim()?.Split(',') ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Naughtylist
        {
            get => this.NaughtylistString?.Trim()?.Split(',') ?? Enumerable.Empty<string>();
        }
    }
}
