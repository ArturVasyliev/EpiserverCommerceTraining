using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using CommerceTraining.Models.Pages;
using CommerceTraining.Models.ViewModels;
using Mediachase.Commerce.Catalog;
using EPiServer.Commerce.Order;
using System;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Customers;
using EPiServer.Commerce.Catalog.ContentTypes;
using CommerceTraining.Models.Catalog;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Commerce.Catalog.Linking;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Pricing;

namespace CommerceTraining.Controllers
{
    public class AdminPageController : PageController<AdminPage>
    {
        private readonly ReferenceConverter _referenceConverter;

        public AdminPageController(
            ReferenceConverter referenceConverter,
            IContentLoader contentLoader,
            AssetUrlResolver assetUrlReslver)
        {
            _referenceConverter = referenceConverter;
        }

        public ActionResult Index(AdminPage currentPage)
        {
            AdminViewModel model = new AdminViewModel();
            ContentReference aRef = _referenceConverter.GetContentLink("Shirts_1");
            ContentReference aRef2 = _referenceConverter.GetContentLink("Men_1");
            ContentReference aRef3 = _referenceConverter.GetContentLink(2, CatalogContentType.CatalogEntry, 0);

            CheckReferenceConverter(aRef, model);
            CheckReferenceConverter(aRef2, model);
            CheckReferenceConverter(aRef3, model);

            return View(model);
        }

        public void CheckReferenceConverter(ContentReference contentReference, AdminViewModel model)
        {
            RefInfo refInfo = new RefInfo();
            refInfo.Ref = contentReference;
            refInfo.Code = _referenceConverter.GetCode(contentReference);
            refInfo.DbId = _referenceConverter.GetObjectId(contentReference);
            refInfo.CatalogType = _referenceConverter.GetContentType(contentReference).ToString();
            refInfo.RootRef = _referenceConverter.GetRootLink();

            model.Refs.Add(refInfo);
        }
    }
}