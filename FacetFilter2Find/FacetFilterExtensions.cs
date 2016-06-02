using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Find;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Api.Querying;
using EPiServer.Find.Helpers;
using EPiServer.Find.Helpers.Reflection;
using FacetFilter2Find.Api.Facets;

namespace FacetFilter2Find
{
    public static class FacetFilterExtensions
    {
        public static ITypeSearch<TSource> TermsFacetFor<TSource>(
            this ITypeSearch<TSource> search,
            Expression<Func<TSource, string>> fieldSelector, Expression<Func<TSource, Filter>> filterExpression, Action<TermsFacetFilterRequest> facetRequestAction = null)
        {
            filterExpression.ValidateNotNullArgument("filterExpression");
            var filterExpressionParser = new FilterExpressionParser(search.Client.Conventions);
            var facetFilter = filterExpressionParser.GetFilter(filterExpression);
            var facetName = fieldSelector.GetFieldPath();
            var fieldName = search.Client.Conventions.FieldNameConvention.GetFieldName(fieldSelector);
            return new Search<TSource, IQuery>(search, context =>
            {
                var facetRequest = new TermsFacetFilterRequest(facetName, facetFilter)
                {
                    Field = fieldName
                };
                if (facetRequestAction.IsNotNull())
                {
                    facetRequestAction(facetRequest);
                }
                context.RequestBody.Facets.Add(facetRequest);
            });
        }
    }
}
