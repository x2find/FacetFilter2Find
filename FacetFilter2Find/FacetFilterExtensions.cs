using System;
using System.Linq.Expressions;
using EPiServer.Find;
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
            Expression<Func<TSource, string>> fieldSelector,
            Expression<Func<TSource, Filter>> filterExpression,
            Action<TermsFacetFilterRequest> facetRequestAction = null)
        {
            filterExpression.ValidateNotNullArgument("filterExpression");
            var filterExpressionParser = new FilterExpressionParser(search.Client.Conventions);
            var facetFilter = filterExpressionParser.GetFilter(filterExpression);
            var facetName = fieldSelector.GetFieldPath();
            return search.TermsFacetFor(
                facetName,
                fieldSelector,
                facetFilter,
                facetRequestAction);
        }

        public static ITypeSearch<TSource> TermsFacetFor<TSource>(
            this ITypeSearch<TSource> search,
            string facetName,
            Expression fieldExpression,
            Filter facetFilter,
            Action<TermsFacetFilterRequest> facetRequestAction = null)
        {
            var fieldName = search.Client.Conventions.FieldNameConvention.GetFieldName(fieldExpression);
            return search.TermsFacetFor(
                facetName,
                fieldName,
                facetFilter,
                facetRequestAction);
        }

        public static ITypeSearch<TSource> TermsFacetFor<TSource>(
            this ITypeSearch<TSource> search,
            string facetName,
            string fieldName,
            Filter facetFilter,
            Action<TermsFacetFilterRequest> facetRequestAction = null)
        {
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
