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
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.InventoryService;
using Mediachase.Commerce.Orders;
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
        private readonly IPromotionEngine _promotionEngine;

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
            IPlacedPriceProcessor placedPriceProcessor,
            IPromotionEngine promotionEngine)
            : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
            _orderRepository = orderRepository;
            _orderFactory = orderFactory;
            _lineItemValidator = lineItemValidator;
            _currentMarket = currentMarket;
            _invService = invService;
            _whRep = whRep;
            _placedPriceProcessor = placedPriceProcessor;
            _promotionEngine = promotionEngine;
        }

        public ActionResult Index(ShirtVariation currentContent)
        {
            LoadingExamples(currentContent);

            // Dont't do like this
            //var myVariable = ServiceLocator.Current
            //    .GetInstance<IPriceDetailService>();

            // Don't use this
            //CatalogContentLoader ccc = new CatalogContentLoader();
            //ccc.GetCatalogEntries()

            var currM = _currentMarket.GetCurrentMarket();
            var link = currentContent.ContentLink;
            //Price price = _readOnlyPricingLoader.Service.GetDefaultPrice(link, currM.MarketId, currM.DefaultCurrency, DateTime.UtcNow);

            // just checking
            //IEnumerable<DiscountedEntry> entries =
            //    _promotionEngine.GetDiscountPrices(
            //    currentContent.ContentLink, _currentMarket.GetCurrentMarket());

            Decimal theDiscount = 0;
            string discountString = String.Empty;

            // ...can also get the price direct by _promotionEngine.GetDiscountPrices(....) (above)
            List<RewardDescription> rewards = new List<RewardDescription>();
            rewards = _promotionEngine.Evaluate(currentContent.ContentLink).ToList();
            if (rewards.Count != 0)
            {
                theDiscount = rewards.First().SavedAmount; // just take one to have a look
                discountString = rewards.First().Description;
                discountString += " : ";
                discountString += rewards.First().Promotion.Description;
            }
            else
            {
                discountString = "...no discount";
            }


            var model = new Models.ViewModels.ShirtVariationViewModel
            {
                MainBody = currentContent.MainBody,
                priceString = currentContent.GetDefaultPrice().UnitPrice.Amount.ToString("C"),

                discountString = discountString,
                discountPrice = currentContent.GetDefaultPrice().UnitPrice.Amount - theDiscount, // could be other than "Default"

                image = GetDefaultAsset(currentContent),
                CanBeMonogrammed = currentContent.CanBeMonogrammed,

            };

            return View(model);
        }

        private void LoadingExamples(ShirtVariation currentContent)
        {
            // AdminPageController have demo of ReferenceConverter

            #region Catalog

            ContentReference parent = currentContent.ParentLink; //...from "me" as the variation
            // note: in 11 --> more strict

            //var x = base._contentLoader.Get<EntryContentBase>(parent);
            var y = base._contentLoader.Get<NodeContentBase>(parent); // gets the ShirtNodeProxy

            IEnumerable<EntryContentBase> children =
                base._contentLoader.GetChildren<EntryContentBase>(parent);

            IEnumerable<ContentReference> allLinks = currentContent.GetCategories(); // Relations
            IEnumerable<Relation> nodes = currentContent.GetNodeRelations(); // older, avoid

            var theType = currentContent.GetOriginalType(); // handy
            var proxy = currentContent.GetType(); // gives the CastleProxy

            IEnumerable<ContentReference> prodParents = currentContent.GetParentProducts();
            IEnumerable<ContentReference> parentPackages = currentContent.GetParentPackages();
            IEnumerable<ContentReference> allParents = currentContent.GetParentEntries(); // newer

            IMarket market = _currentMarket.GetCurrentMarket(); // Gets "DEFAULT" if not "custom"
            bool available = currentContent.IsAvailableInMarket(market.MarketId); // if we want to know about another market
            bool available2 = currentContent.IsAvailableInCurrentMarket();

            //ISecurityDescriptor sec = currentContent.GetSecurityDescriptor();

            CatalogEntryResponseGroup respG = new CatalogEntryResponseGroup(
                   CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull);

            // Finally in 12 we get squiggles :)
            // old school, not needed after 9.19 ... avoid. 
            // ...this is why we have the extension method
            //Mediachase.Commerce.Catalog.Objects.Entry entry =
            //    currentContent.LoadEntry(); // Consider RG

            // the IoC-way to get the above, but use the .ctor not do like this...
            ICatalogSystem catSys = ServiceLocator.Current.GetInstance<ICatalogSystem>();
            // catSys

            //Entry shouldNotUseThis = catSys.GetCatalogEntry(2, respG);
            //var p = shouldNotUseThis.PriceValues; // still populating from price-service, from ECF R3

            // native ECF, just to have a look, still used a lot ... can be handy
            var entryDto = CatalogContext.Current.GetCatalogEntryDto // singular
                (currentContent.Code, respG);
            var x = entryDto.SalePrice; // not used anymore, zero prices back

            // used a lot previously, in combination with the search provider model
            // we can use "search" to easily get an array of "ints" back.
            var entryDto2 = CatalogContext.Current.GetCatalogEntriesDto // Plural, check the overloads
                (new int[] { 2, 3, 4, 5, 6 }, respG);

            #endregion


            #region Orders

            var p0 = OrderContext.Current.FindActiveOrders(); // InProgress & Partially shipped
            //OrderContext.Current.FindPurchaseOrders(); // Not fun... ref Shannons blog (carts too)
            var p1 = OrderContext.Current.FindPurchaseOrdersByStatus(OrderStatus.AwaitingExchange); // array of statuses as arg
            var p2 = OrderContext.Current.GetPurchaseOrders(new Guid()); // ContactGuid
            var p3 = OrderContext.Current.GetPurchaseOrder(-1); // TrackingNo or DB-PK

            var po0 = _orderRepository.Load(); // all for current user - only POs
            var po1 = _orderRepository.Load(CustomerContext.Current.CurrentContactId); // Only POs
            var po2 = _orderRepository.Load(CustomerContext.Current.CurrentContactId, "WishList");
            var po3 = _orderRepository.Load<ICart>();
            var po4 = _orderRepository.Load<IPurchaseOrder>(21);

            #endregion

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