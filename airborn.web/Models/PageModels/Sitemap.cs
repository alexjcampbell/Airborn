using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Airborn.web.Models
{

    public class SitemapUrl
    {
        public string Url { get; set; }
        public DateTime LastModified { get; set; }
        public ChangeFrequency ChangeFrequency { get; set; }
        public double Priority { get; set; }
    }

    public enum ChangeFrequency
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }


    public class SitemapGenerator
    {
        private readonly IUrlHelper _urlHelper;

        public SitemapGenerator(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string GenerateSitemap(IEnumerable<SitemapUrl> urls)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    urls.Select(url => new XElement(ns + "url",
                        new XElement(ns + "loc", url.Url),
                        new XElement(ns + "lastmod", url.LastModified.ToString("yyyy-MM-ddTHH:mm:sszzz")),
                        new XElement(ns + "changefreq", url.ChangeFrequency.ToString().ToLower()),
                        new XElement(ns + "priority", url.Priority.ToString("F1"))
                    ))
                )
            );

            var stringBuilder = new StringBuilder();
            using (var writer = new Utf8StringWriter(stringBuilder))
            {
                sitemap.Save(writer);
            }

            return stringBuilder.ToString();
        }


        public string GenerateSitemapIndex(IEnumerable<SitemapUrl> sitemapUrls)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var xml = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "sitemapindex",
                    from sitemapUrl in sitemapUrls
                    select new XElement(ns + "sitemap",
                        new XElement(ns + "loc", sitemapUrl.Url),
                        new XElement(ns + "lastmod", sitemapUrl.LastModified.ToString("yyyy-MM-dd"))
                    )
                )
            );

            return xml.ToString();
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter(StringBuilder sb) : base(sb) { }

        public override Encoding Encoding => Encoding.UTF8;

    }
}
