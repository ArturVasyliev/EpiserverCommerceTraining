using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Commerce;

namespace CommerceTraining.Models.Blocks
{
    [ContentType(DisplayName = "SettingsBlock", GUID = "fdfd33be-91ca-4366-a3ac-ea126c66f0e7", Description = "")]
    public class SettingsBlock : BlockData
    {
        [UIHint(UIHint.CatalogContent)]
        public virtual ContentReference TopCategory { get; set; }

        public virtual ContentReference CartPage { get; set; }

        public virtual ContentReference CheckoutPage { get; set; }

        public virtual ContentReference OrderPage { get; set; }

        public virtual ContentReference CatalogStartPageLink { get; set; }

        //public virtual bool InitializedOrNot { get; set; } wait with this
    }
}