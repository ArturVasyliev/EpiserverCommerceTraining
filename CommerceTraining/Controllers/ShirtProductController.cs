using CommerceTraining.Models.Catalog;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Routing;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    [TemplateDescriptor(Default =true)]
    public class ShirtProductController : CatalogControllerBase<ShirtProduct>
    {
        public ShirtProductController(
            IContentLoader contentLoader,
            UrlResolver urlResolver,
            AssetUrlResolver assetUrlResolver,
            ThumbnailUrlResolver thumbnailUrlResolver)
            : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        public ActionResult Index(ShirtProduct currentContent)
        {
            return View(currentContent);
        }
    }
}