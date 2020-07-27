using CommerceTraining.Models.Catalog;
using EPiServer.Core;
using System.Collections.Generic;

namespace CommerceTraining.Models.ViewModels
{
    public class SearchResultViewModel
    {
        // no paging settings, only simple setup

        public IEnumerable<string> totalHits { get; set; }
        public IEnumerable<FashionNode> nodes { get; set; }
        public IEnumerable<ShirtProduct> products { get; set; }
        public IEnumerable<ShirtVariation> variants { get; set; }
        public IEnumerable<IContent> allContent { get; set; }
        public IEnumerable<string> facets { get; set; }

    }
}