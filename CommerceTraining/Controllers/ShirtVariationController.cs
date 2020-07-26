using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.Pages;
using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.InventoryService;
using Mediachase.Commerce.Security; // for ext-m. on CurrentPrincipal

namespace CommerceTraining.Controllers
{
    public class ShirtVariationController : CatalogControllerBase<ShirtVariation>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderGroupFactory _orderFactory;
        private readonly ILineItemValidator _lineItemValidator;
        private readonly ICurrentMarket _currentMarket;
        private readonly IInventoryService _invService;
        private readonly IWarehouseRepository _whRep;
        private readonly IPlacedPriceProcessor _placedPriceProcessor;

        public ShirtVariationController(
            IContentLoader contentLoader,
            UrlResolver urlResolver,
            AssetUrlResolver assetUrlResolver,
            ThumbnailUrlResolver thumbnailUrlResolver,
            IOrderRepository orderRepository,
            IOrderGroupFactory orderFactory,
            ILineItemValidator lineItemValidator,
            ICurrentMarket currentMarket,
            IInventoryService invService,
            IWarehouseRepository whRep,
            IPlacedPriceProcessor placedPriceProcessor)
            : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
            _orderRepository = orderRepository;
            _orderFactory = orderFactory;
            _lineItemValidator = lineItemValidator;
            _currentMarket = currentMarket;
            _invService = invService;
            _whRep = whRep;
            _placedPriceProcessor = placedPriceProcessor;
        }

        public ActionResult Index(ShirtVariation currentContent)
        {
            var model = new ShirtVariationViewModel
            {
                MainBody = currentContent.MainBody,
                priceString = currentContent.GetDefaultPrice().UnitPrice.Amount.ToString("C"),
                image = GetDefaultAsset(currentContent),
                CanBeMonogrammed = currentContent.CanBeMonogrammed,
            };

            return View(model);
        }

        public ActionResult AddToCart(ShirtVariation currentContent, decimal Quantity, string Monogram)
        {
            // LoadOrCreateCart - in EPiServer.Commerce.Order.IOrderRepositoryExtensions
            var cart = _orderRepository.LoadOrCreateCart<ICart>(
                PrincipalInfo.CurrentPrincipal.GetContactId(), "Default");

            //cart.  // ...have a look at all the extension methods

            string code = currentContent.Code;

            // Manually checking - could have a look at the InventoryRecord - properties
            // ...could be part of an optional lab in Fund
            IWarehouse wh = _whRep.GetDefaultWarehouse();
            InventoryRecord rec = _invService.Get(code, wh.Code);


            // ... can get: Sequence contains more than one matching element...
            // ...when we have two different lineitems for the same SKU 
            // Use when the cart is empty/one LI for SKU with Qty --> no crash
            var lineItem = cart.GetAllLineItems().SingleOrDefault(x => x.Code == code);

            //currentContent.IsAvailableInCurrentMarket();

            // Below works for the same SKU on different LineItems... Changing back for the Fund
            //var lineItem = cart.GetAllLineItems().First(x => x.Code == code); // Changed to this for multiple LI with the same code - crash if no cart

            // ECF 12 changes - market.MarketId
            IMarket market = _currentMarket.GetCurrentMarket();

            if (lineItem == null)
            {
                lineItem = _orderFactory.CreateLineItem(code, cart);
                lineItem.Quantity = Quantity; // gets this as an argument for the method

                // ECF 12 changes - market.MarketId
                _placedPriceProcessor.UpdatePlacedPrice
                    (lineItem, GetContact(), market.MarketId, cart.Currency,
                    (lineItemToValidate, validation) => { }); // does not take care of the messages here 

                cart.AddLineItem(lineItem);
            }
            else
            {
                // Qty increases ... no new LineItem ... 
                // original for Fund.
                lineItem.Quantity += Quantity; // need an update
                // maybe do price validation here too

            }

            // Validations
            var validationIssues = new Dictionary<ILineItem, ValidationIssue>();

            // ECF 12 changes - market.MarketId
            // RoCe - This needs to be updated to get the message out + Lab-steps added...
            var validLineItem = _lineItemValidator.Validate(lineItem, market.MarketId, (item, issue) => { });
            cart.ValidateOrRemoveLineItems((item, issue) => validationIssues.Add(item, issue), _lineItemValidator);
            //var someIssue = validationIssues.First().Value;

            if (validLineItem) // We're happy
            {
                // If MDP - error when adding, may need to reset IIS as the model has changed 
                //    when adding/removing the MetaField in CM
                lineItem.Properties["Monogram"] = Monogram;

                _orderRepository.Save(cart);
            }

            ContentReference cartRef = _contentLoader.Get<StartPage>(ContentReference.StartPage).Settings.CartPage;
            ContentReference cartPageRef = EPiServer.Web.SiteDefinition.Current.StartPage;
            CartPage cartPage = _contentLoader.Get<CartPage>(cartRef);
            var name = cartPage.Name;
            var lang = ContentLanguage.PreferredCulture;

            string passingValue = cart.Name; // if something is needed
            //return RedirectToAction("Index", new { node = theRef, passedAlong = passingValue }); // Doesn't work
            return RedirectToAction("Index", lang + "/" + name, new { passedAlong = passingValue }); // Works
        }

        public void AddToWishList(ShirtVariation currentContent)
        {
            ICart myWishList = _orderRepository.LoadOrCreateCart<ICart>
                (PrincipalInfo.CurrentPrincipal.GetContactId(), "WishList");

            ILineItem lineItem = _orderFactory.CreateLineItem(currentContent.Code, myWishList);
            myWishList.AddLineItem(lineItem);
            _orderRepository.Save(myWishList);
        }

        protected static CustomerContact GetContact()
        {
            return CustomerContext.Current.GetContactById(GetContactId());
        }

        protected static Guid GetContactId()
        {
            return PrincipalInfo.CurrentPrincipal.GetContactId();
        }
    }
}