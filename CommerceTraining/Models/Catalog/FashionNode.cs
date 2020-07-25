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
        DisplayName = "FashionNode",
        MetaClassName = "Fashion_Node",
        GUID = "e78d4f6c-0119-4f84-afb7-12c228b6ce51",
        Description = "")]
    public class FashionNode : NodeContent
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
        // MainBody property is also present in ShirtProduct model.
        // Because of some commerce restrictions, they should 
        // be similar (have the same attributes) in order to work.
        // If they should be different, then just rename one of them.
        public virtual XhtmlString MainBody { get; set; }
    }
}