using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Provides functions to access robots.txt file.
    /// </summary>
    public static class RobotsTextHelper {
        public const string URL = "~/robots.txt";
        public const string SITEMAP_KEY = "Sitemap";

        /// <summary>
        /// Gets a value to indicate whether the robots.txt file is existing.
        /// </summary>
        /// <returns></returns>
        public static bool Exists() {
            return File.Exists(HostingEnvironment.MapPath(URL));
        }

        /// <summary>
        /// Gets content of robots.txt file.
        /// </summary>
        /// <returns></returns>
        public static string GetContent() {
            string content = string.Empty;
            string path = HostingEnvironment.MapPath(URL);

            if(!File.Exists(path)) {
                File.WriteAllText(path, string.Empty, Encoding.UTF8);
            } else {
                content = File.ReadAllText(path, Encoding.UTF8);
            }

            return content;
        }

        /// <summary>
        /// Sets content of robots.txt file.
        /// </summary>
        /// <param name="content"></param>
        public static void SetContent(string content) {
            string path = HostingEnvironment.MapPath(URL);
            File.WriteAllText(path, content ?? string.Empty, Encoding.UTF8);
        }

        /// <summary>
        /// Appends a sitemap index to robots.txt file.
        /// </summary>
        /// <param name="url"></param>
        public static void AppendSitemapIndex(string url) {
            if(string.IsNullOrWhiteSpace(url)) {
                throw new ArgumentException("sitemap index url is null or empty.", "url");
            }

            string path = HostingEnvironment.MapPath(URL);
            string sitemap = string.Format("{0}: {1}", SITEMAP_KEY, url);
            if(File.Exists(path)) {
                string[] lines = File.ReadAllLines(path, Encoding.UTF8).Where((item) => !item.StartsWith(SITEMAP_KEY, StringComparison.OrdinalIgnoreCase)).Concat(new string[] { sitemap }).ToArray();
                File.WriteAllLines(path, lines, Encoding.UTF8);
            } else {
                File.WriteAllText(path, sitemap, Encoding.UTF8);
            }
        }
    }
}
