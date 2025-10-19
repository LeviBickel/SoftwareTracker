using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml;

namespace SoftwareTracker.Controllers
{
    public class SitemapController : Controller
    {
        [Route("sitemap.xml")]
        [ResponseCache(Duration = 86400)] // Cache for 24 hours
        public IActionResult Index()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var sitemap = new StringBuilder();
            sitemap.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sitemap.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Home page
            AddUrl(sitemap, baseUrl, "", "1.0", "daily", DateTime.UtcNow);

            // Privacy page
            AddUrl(sitemap, baseUrl, "/Home/Privacy", "0.5", "monthly", DateTime.UtcNow.AddDays(-7));

            sitemap.AppendLine("</urlset>");

            return Content(sitemap.ToString(), "application/xml", Encoding.UTF8);
        }

        private void AddUrl(StringBuilder sitemap, string baseUrl, string path, string priority, string changeFreq, DateTime lastMod)
        {
            sitemap.AppendLine("  <url>");
            sitemap.AppendLine($"    <loc>{baseUrl}{path}</loc>");
            sitemap.AppendLine($"    <lastmod>{lastMod:yyyy-MM-dd}</lastmod>");
            sitemap.AppendLine($"    <changefreq>{changeFreq}</changefreq>");
            sitemap.AppendLine($"    <priority>{priority}</priority>");
            sitemap.AppendLine("  </url>");
        }
    }
}
