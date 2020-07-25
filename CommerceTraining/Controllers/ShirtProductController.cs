using CommerceTraining.Models.Catalog;
using EPiServer.Framework.DataAnnotations;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    [TemplateDescriptor(Default =true)]
    public class ShirtProductController : CatalogControllerBase<ShirtProduct>
    {
        public ShirtProductController()
            : base()
        {
        }

        public ActionResult Index(ShirtProduct currentContent)
        {
            return View(currentContent);
        }
    }
}