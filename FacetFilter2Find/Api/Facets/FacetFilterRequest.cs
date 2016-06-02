using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Api.Querying;
using EPiServer.Find.Helpers;

namespace FacetFilter2Find.Api.Facets
{
    public class FacetFilterRequest : FacetRequest
    {
        public FacetFilterRequest(string name, Filter facetFilter)
            : base(name)
        {
            facetFilter.ValidateNotNullArgument("facetFilter");
            this.FacetFilter = facetFilter;
        }

        public Filter FacetFilter { get; set; }
    }
}
