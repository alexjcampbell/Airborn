using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Airborn.web.Models;
using System.Linq;


namespace Airborn.web.Controllers
{
    public class SitemapController : Controller
    {
        private readonly ILogger<SitemapController> _logger;
        private readonly AirbornDbContext _dbContext;
        private IWebHostEnvironment _env;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        public SitemapController(ILogger<SitemapController> logger, IWebHostEnvironment env, AirbornDbContext dbContext, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _logger = logger;
            _env = env;
            _dbContext = dbContext;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
        }

        [Route("sitemap.xml")]
        public IActionResult Index()
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var sitemapGenerator = new SitemapGenerator(Url);
            var sitemapUrls = _dbContext.Regions.AsNoTracking()
                .Select(r => new SitemapUrl
                {
                    Url = Url.Action("RegionAirportsSitemap", "Sitemap", new { countrySlug = r.Country.Slug, regionSlug = r.Slug }, Url.ActionContext.HttpContext.Request.Scheme),
                    LastModified = DateTime.UtcNow,
                    ChangeFrequency = ChangeFrequency.Daily,
                    Priority = 1.0
                }).ToList<SitemapUrl>();

            sitemapUrls.Add(new SitemapUrl
            {
                Url = urlHelper.Action("GetSitemapUrlsForEverythingOtherThanAirports", "Sitemap", values: null, protocol: urlHelper.ActionContext.HttpContext.Request.Scheme),
                LastModified = DateTime.UtcNow,
                ChangeFrequency = ChangeFrequency.Daily,
                Priority = 1.0
            });

            var sitemapIndex = sitemapGenerator.GenerateSitemapIndex(sitemapUrls);

            return Content(sitemapIndex, "application/xml", Encoding.UTF8);
        }


        public IActionResult GetSitemapUrlsForEverythingOtherThanAirports()
        {
            List<SitemapUrl> sitemapUrls = new List<SitemapUrl>();
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var sitemapGenerator = new SitemapGenerator(Url);

            sitemapUrls.Add(new SitemapUrl
            {
                Url = urlHelper.AbsoluteRouteUrl("default", new { controller = "Home", action = "Index" }),
                LastModified = DateTime.UtcNow,
                ChangeFrequency = ChangeFrequency.Daily,
                Priority = 1.0
            });

            sitemapUrls.Add(new SitemapUrl
            {
                Url = urlHelper.AbsoluteRouteUrl("default", new { controller = "Home", action = "About" }),
                LastModified = DateTime.UtcNow,
                ChangeFrequency = ChangeFrequency.Daily,
                Priority = 1.0
            });

            sitemapUrls.Add(new SitemapUrl
            {
                Url = urlHelper.AbsoluteRouteUrl("default", new { controller = "Home", action = "Terms" }),
                LastModified = DateTime.UtcNow,
                ChangeFrequency = ChangeFrequency.Daily,
                Priority = 1.0
            });

            sitemapUrls.Add(new SitemapUrl
            {
                Url = urlHelper.AbsoluteRouteUrl("default", new { controller = "Airports", action = "Index" }),
                LastModified = DateTime.UtcNow,
                ChangeFrequency = ChangeFrequency.Daily,
                Priority = 1.0
            });

            sitemapUrls.Add(new SitemapUrl
            {
                Url = urlHelper.AbsoluteRouteUrl("default", new { controller = "Airplanes", action = "Index" }),
                LastModified = DateTime.UtcNow,
                ChangeFrequency = ChangeFrequency.Daily,
                Priority = 1.0
            });

            foreach (var continent in _dbContext.Continents.AsNoTracking())
            {
                sitemapUrls.Add(new SitemapUrl
                {
                    Url = urlHelper.Action("Continent", "Airports", new { slug = continent.Slug }, urlHelper.ActionContext.HttpContext.Request.Scheme),
                    LastModified = DateTime.UtcNow,
                    ChangeFrequency = ChangeFrequency.Daily,
                    Priority = 1.0
                });
            }

            foreach (var country in _dbContext.Countries.AsNoTracking())
            {
                sitemapUrls.Add(new SitemapUrl
                {
                    Url = urlHelper.Action("Country", "Airports", new { slug = country.Slug }, urlHelper.ActionContext.HttpContext.Request.Scheme),
                    LastModified = DateTime.UtcNow,
                    ChangeFrequency = ChangeFrequency.Daily,
                    Priority = 1.0
                });
            }

            return Content(sitemapGenerator.GenerateSitemap(sitemapUrls), "application/xml", Encoding.UTF8);
        }


        [Route("Sitemap/{countrySlug}/{regionSlug}.xml")]
        public IActionResult RegionAirportsSitemap(string countrySlug, string regionSlug)
        {
            var sitemapGenerator = new SitemapGenerator(Url);
            var region = _dbContext.Regions.AsNoTracking().FirstOrDefault(r => r.Slug == regionSlug && r.Country.Slug == countrySlug);

            if (region == null)
            {
                return NotFound();
            }

            var urls = _dbContext.Airports.AsNoTracking()
                .Where(a => a.Region.Slug == regionSlug && a.Region.Country.Slug == countrySlug)
                .Select(a => new SitemapUrl
                {
                    Url = Url.Action("Airport", "Airports", new { ident = a.Ident }, Url.ActionContext.HttpContext.Request.Scheme),
                    LastModified = a.LastUpdated,
                    ChangeFrequency = ChangeFrequency.Daily,
                    Priority = 1.0
                });

            var sitemap = sitemapGenerator.GenerateSitemap(urls);
            return Content(sitemap, "application/xml", Encoding.UTF8);
        }

    }


    public static class UrlHelperExtensions
    {
        public static string AbsoluteRouteUrl(this IUrlHelper urlHelper, string routeName, object routeValues = null)
        {
            var scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            return urlHelper.RouteUrl(routeName, routeValues, scheme);
        }
    }
}