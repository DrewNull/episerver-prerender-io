// ReSharper disable StringLiteralTypo
namespace PrerenderForEpiserver
{
    using System.Collections.Generic;

    public static class PrerenderConstants
    {
        public const string EscapedFragmentQueryParameter = "_escaped_fragment_";
        public const string ServiceUrl = "http://service.prerender.io";
        public const string TokenAppSetting = "Prerender:Token";

        public static readonly List<string> CrawlerUserAgents = new List<string>
        {
            "Applebot",
            "baiduspider",
            "bingbot",
            "bitlybot",
            "developers.google.com/+/web/snippet",
            "Discordbot",
            "embedly",
            "facebookexternalhit",
            "flipboard",
            "Google Page Speed",
            "googlebot",
            "linkedinbot",
            "nuzzel",
            "outbrain",
            "pinterest/0.",
            "quora link preview",
            "redditbot",
            "rogerbot",
            "showyoubot",
            "SkypeUriPreview",
            "slackbot",
            "tumblr",
            "twitterbot",
            "vkShare",
            "W3C_Validator",
            "WhatsApp",
            "x-bufferbot",
            "yahoo",
            "yandex"
        };

        public static readonly List<string> ExtensionsToIgnore = new List<string>
        {
            ".ai",
            ".avi",
            ".axd",
            ".css",
            ".dat",
            ".dmg",
            ".doc",
            ".doc",
            ".exe",
            ".flv",
            ".gif",
            ".iso",
            ".jpeg",
            ".jpg",
            ".js",
            ".less",
            ".m4a",
            ".m4v",
            ".mov",
            ".mp3",
            ".mp4",
            ".mpeg",
            ".mpg",
            ".pdf",
            ".png",
            ".ppt",
            ".psd",
            ".rar",
            ".swf",
            ".tif",
            ".torrent",
            ".txt",
            ".wav",
            ".wmv",
            ".xls",
            ".zip",
        };
    }
}