using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find.Api;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Helpers;
using EPiServer.Find.Helpers.Text;
using Newtonsoft.Json;

namespace FacetFilter2Find.Api.Facets
{
    public class TermsFacetFilterRequestConverter : JsonConverter
    {
        private const int MinSize = 0;
        private const int MaxSize = 1000;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var facetRequest = (TermsFacetFilterRequest)value;
            writer.WriteStartObject();
            writer.WritePropertyName("terms");
            writer.WriteStartObject();
            if (facetRequest.Field.IsNotNullOrEmpty())
            {
                writer.WritePropertyName("field");
                writer.WriteValue(facetRequest.Field);
            }
            if (facetRequest.Fields.IsNotNull() && facetRequest.Fields.Any())
            {
                writer.WritePropertyName("fields");
                writer.WriteStartArray();
                foreach (var field in facetRequest.Fields)
                {
                    serializer.Serialize(writer, field);
                }
                writer.WriteEndArray();
            }
            if (facetRequest.Script.IsNotNullOrEmpty())
            {
                writer.WritePropertyName("script");
                writer.WriteValue(facetRequest.Script);
            }
            if (facetRequest.AllTerms)
            {
                writer.WritePropertyName("all_terms");
                writer.WriteValue(true);
            }
            if (facetRequest.Size.HasValue)
            {
                if (facetRequest.Size.Value < MinSize)
                {
                    throw new InvalidSearchRequestException(string.Format(CultureInfo.InvariantCulture, "Terms facet size can not be set to a lower value than 0. Current value: '{0}'", facetRequest.Size.Value));
                }

                if (facetRequest.Size.Value > MaxSize)
                {
                    throw new InvalidSearchRequestException(string.Format(CultureInfo.InvariantCulture, "Terms facet size can not be set to a higher value than 1000. Current value: '{0}'", facetRequest.Size.Value));
                }

                writer.WritePropertyName("size");
                writer.WriteValue(facetRequest.Size.Value);
            }
            writer.WriteEndObject();
            if (facetRequest.FacetFilter.IsNotNull())
            {
                writer.WritePropertyName("facet_filter");
                serializer.Serialize(writer, facetRequest.FacetFilter);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TermsFacetFilterRequest).IsAssignableFrom(objectType);
        }
    }
}
