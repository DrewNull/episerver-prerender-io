namespace PrerenderForEpiserver
{
    using System.Collections.Generic;
    using System.Configuration;

    public sealed class PrerenderConfigSection : ConfigurationSection
    {
        public const string DefaultPrerenderServiceUrl = "http://service.prerender.io";

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

        [ConfigurationProperty("stripApplicationNameFromRequestUrl", DefaultValue = false)]
        public bool StripApplicationNameFromRequestUrl
        {
            get => (bool)this["stripApplicationNameFromRequestUrl"];
            set => this["stripApplicationNameFromRequestUrl"] = value;
        }

        [ConfigurationProperty("nicelist")]
        public string NicelistString
        {
            get => (string)this["nicelist"];
            set => this["nicelist"] = value;
        }

        public IEnumerable<string> Nicelist =>
            string.IsNullOrWhiteSpace(this.NicelistString)
                ? null
                : this.NicelistString.Trim().Split(',');

        [ConfigurationProperty("naughtylist")]
        public string NaughtylistString
        {
            get => (string)this["naughtylist"];
            set => this["naughtylist"] = value;
        }

        public IEnumerable<string> Naughtylist =>
            string.IsNullOrWhiteSpace(this.NaughtylistString)
                ? null
                : this.NaughtylistString.Trim().Split(',');

        [ConfigurationProperty("extensionsToIgnore")]
        public string ExtensionsToIgnoreString
        {
            get => (string)this["extensionsToIgnore"];
            set => this["extensionsToIgnore"] = value;
        }

        public IEnumerable<string> ExtensionsToIgnore =>
            string.IsNullOrWhiteSpace(this.ExtensionsToIgnoreString)
                ? null
                : this.ExtensionsToIgnoreString.Trim().Split(',');

        [ConfigurationProperty("crawlerUserAgents")]
        public string CrawlerUserAgentsString
        {
            get => (string)this["crawlerUserAgents"];
            set => this["crawlerUserAgents"] = value;
        }

        public IEnumerable<string> CrawlerUserAgents =>
            string.IsNullOrWhiteSpace(this.CrawlerUserAgentsString)
                ? null
                : this.CrawlerUserAgentsString.Trim().Split(',');

        [ConfigurationProperty("Proxy")]
        public ProxyConfigElement Proxy
        {
            get => (ProxyConfigElement)this["Proxy"];
            set => this["Proxy"] = value;
        }

        [ConfigurationProperty("token")]
        public string Token
        {
            get => (string)this["token"];
            set => this["token"] = value;
        }
    }
}
