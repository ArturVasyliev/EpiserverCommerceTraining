using CommerceTraining.Models.Catalog;
using EPiServer.Framework.DataAnnotations;

namespace CommerceTraining.Controllers
{
    [TemplateDescriptor(Default =true)]
    public class ShirtProductController : CatalogControllerBase<ShirtProduct>
    {
        public ShirtProductController()
            : base()
        {
        }
    }
}