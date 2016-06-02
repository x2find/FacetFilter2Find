using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Api.Querying;
using EPiServer.Find.Helpers;
using Newtonsoft.Json;

namespace FacetFilter2Find.Api.Facets
{
    [JsonConverter(typeof(TermsFacetFilterRequestConverter))]
    public class TermsFacetFilterRequest : FacetFilterRequest
    {
        public TermsFacetFilterRequest(string name, Filter facetFilter)
            : base(name, facetFilter)
        {
        }

        public string Field { get; set; }

        public IEnumerable<string> Fields { get; set; }

        public int? Size { get; set; }

        public string Script { get; set; }

        public bool AllTerms { get; set; }
    }
}
