﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceTraining.Models.Catalog;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Security; // for ext-m. on CurrentPrincipal

namespace CommerceTraining.Controllers
{
    public class ShirtVariationController : CatalogControllerBase<ShirtVariation>
    {
        public ShirtVariationController(
            IContentLoader contentLoader,
            UrlResolver urlResolver,
            AssetUrlResolver assetUrlResolver,
            ThumbnailUrlResolver thumbnailUrlResolver)
            : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        public ActionResult Index(ShirtVariation currentContent)
        {
            return View(currentContent);
        }
    }
}