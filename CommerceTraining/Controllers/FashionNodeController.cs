using CommerceTraining.Models.Catalog;
using CommerceTraining.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommerceTraining.Controllers
{
    public class FashionNodeController : CatalogControllerBase<FashionNode>
    {
        public FashionNodeController() 
            : base()
        {
        }

        public ActionResult Index(FashionNode currentContent)
        {
            return View(currentContent);
        }
    }
}