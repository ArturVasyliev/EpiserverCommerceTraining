using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace CommerceTraining.Models.Catalog
{
    [CatalogContentType(
        DisplayName = "ShirtProduct",
        GUID = "7737fd06-ecb3-4cc8-841b-e11de84361c3",
        Description = "")]
    public class ShirtProduct : ProductContent
    {
        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [IncludeInDefaultSearch]
        [Searchable]
        [Tokenize]
        public virtual XhtmlString MainBody { get; set; }

        public virtual string ClothesType { get; set; }

        public virtual string Brand { get; set; }
    }
}