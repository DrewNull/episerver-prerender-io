namespace PrerenderForEpiserver
{
    using System.Configuration;

    public class ProxyConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("url")]
        public string Url
        {
            get
            {
                return (string)this["url"];
            }

            set
            {
                this["url"] = value;
            }
        }

        [ConfigurationProperty("port", DefaultValue = 80)]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }

            set
            {
                this["port"] = value;
            }
        }
    }
}
