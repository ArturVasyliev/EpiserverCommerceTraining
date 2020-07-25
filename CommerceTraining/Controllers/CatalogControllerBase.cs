using CommerceTraining.Models.Media;
using CommerceTraining.Models.Pages;
using CommerceTraining.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog; // AssetUrlResolver
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Security;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceTraining.Controllers
{
    public class CatalogControllerBase<T> 
        : ContentController<T> where T : CatalogContentBase
    {

    }
}