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
        DisplayName = "ShirtVariation", 
        MetaClassName = "Shirt_Variation",
        GUID = "5ce2e54c-ab79-491f-a2d7-1f73b11904da", 
        Description = "")]
    public class ShirtVariation : VariationContent
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

        [IncludeInDefaultSearch]
        public virtual string Size { get; set; }

        [IncludeInDefaultSearch]
        public virtual string Color { get; set; }

        public virtual bool CanBeMonogrammed { get; set; }

        [Searchable]
        [Tokenize]
        [IncludeValuesInSearchResults]
        public virtual string ThematicTag { get; set; }
    }
}