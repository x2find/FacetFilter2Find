using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EPiServer.Find;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Helpers.Reflection;
using FluentAssertions;
using StoryQ;
using Xunit;
using FacetFilter2Find;

namespace FacetFilter2Find.Tests.Stories
{
    public class TermsFacetFilter : IDisposable
    {
        [Fact]
        public void FacetFilter()
        {
            new Story("Terms facet with filter")
                .InOrderTo("be able to create a facet on a single field on a subset of the query result")
                .AsA("developer")
                .IWant("to be able to create a facet and specify a filter for the facet")
                .WithScenario("filter facet")
                .Given(IHaveAClient)
                    .And(IHaveASetOfDocumentsInDifferentCategories)
                    .And(IHaveIndexedTheDocuments)
                    .And(IHaveWaitedFor5Seconds)
                .When(ISearchDocumentsAndSpecifyATermsFacetForCategoryForDocumentsOfTypePdf)
                .Then(IShouldGetATermsFacetForCategoryForDocumentsOfTypePdf)
                .Execute();
        }

        private IClient client;
        void IHaveAClient()
        {
            client = Client.CreateFromConfig();
        }

        private List<Document> documents;
        void IHaveASetOfDocumentsInDifferentCategories()
        {
            documents = new List<Document>();

            for (int i = 0; i < 5; i++)
            {
                var document = new Document() { Name = "doc" + i, Category = "doc_category" + i, Type = "doc" };
                documents.Add(document);
            }

            for (int i = 0; i < 5; i++)
            {
                var document = new Document() { Name = "pdf" + i, Category = "pdf_category" + i, Type = "pdf" };
                documents.Add(document);
            }
        }

        void IHaveIndexedTheDocuments()
        {
            client.Index(documents);
        }

        void IHaveWaitedFor5Seconds()
        {
            // wait for indexed objects to be searchable
            Thread.Sleep(5000);
        }

        private SearchResults<Document> result;
        void ISearchDocumentsAndSpecifyATermsFacetForCategoryForDocumentsOfTypePdf()
        {
            result = client.Search<Document>()
                        .TermsFacetFor(x => x.Category, x => x.Type.Match("pdf"))
                        .GetResult();
        }

        void IShouldGetATermsFacetForCategoryForDocumentsOfTypePdf()
        {
            result.TermsFacetFor(x => x.Category).Terms.Should().OnlyContain(x => x.Term.StartsWith("pdf_category"));
        }

        public void Dispose()
        {
            client.Delete<Document>(x => x.Name.Exists());
        }
    }

    public class MultipleFacetFilter : IDisposable
    {
        [Fact]
        public void FacetFilter()
        {
            new Story("Multiple terms facets with filter")
                .InOrderTo("be able to create a multiple facets on a single field on different subset of the query result")
                .AsA("developer")
                .IWant("to be able to create a facet and specify a filter and a custom name for the facet")
                .WithScenario("filter facet")
                .Given(IHaveAClient)
                    .And(IHaveASetOfDocumentsInDifferentCategories)
                    .And(IHaveIndexedTheDocuments)
                    .And(IHaveWaitedFor5Seconds)
                .When(ISearchDocumentsAndSpecifyATermsFacetForCategoryForDocumentsOfTypePdfAndForDocumentsOfCategoryDoc)
                .Then(IShouldGetATermsFacetForCategoryForDocumentsOfTypePdf)
                    .And(IShouldGetATermsFacetForCategoryForDocumentsOfTypeDoc)
                .Execute();
        }

        private IClient client;
        void IHaveAClient()
        {
            client = Client.CreateFromConfig();
        }

        private List<Document> documents;
        void IHaveASetOfDocumentsInDifferentCategories()
        {
            documents = new List<Document>();

            for (int i = 0; i < 5; i++)
            {
                var document = new Document() { Name = "doc" + i, Category = "doc_category" + i, Type = "doc" };
                documents.Add(document);
            }

            for (int i = 0; i < 5; i++)
            {
                var document = new Document() { Name = "pdf" + i, Category = "pdf_category" + i, Type = "pdf"};
                documents.Add(document);
            }
        }

        void IHaveIndexedTheDocuments()
        {
            client.Index(documents);
        }

        void IHaveWaitedFor5Seconds()
        {
            // wait for indexed objects to be searchable
            Thread.Sleep(5000);
        }

        private SearchResults<Document> result;
        void ISearchDocumentsAndSpecifyATermsFacetForCategoryForDocumentsOfTypePdfAndForDocumentsOfCategoryDoc()
        {
            result = client.Search<Document>()
                        .TermsFacetFor(x => x.Category, x => x.Type.Match("pdf"), x => x.Name = "PdfCategories")
                        .TermsFacetFor(x => x.Category, x => x.Type.Match("doc"), x => x.Name = "DocCategories")
                        .GetResult();
        }

        void IShouldGetATermsFacetForCategoryForDocumentsOfTypePdf()
        {
            (result.Facets["PdfCategories"] as TermsFacet).Terms.Should().OnlyContain(x => x.Term.StartsWith("pdf_category"));
        }

        void IShouldGetATermsFacetForCategoryForDocumentsOfTypeDoc()
        {
            (result.Facets["DocCategories"] as TermsFacet).Terms.Should().OnlyContain(x => x.Term.StartsWith("doc_category"));
        }

        public void Dispose()
        {
            client.Delete<Document>(x => x.Name.Exists());
        }
    }

    public class Document
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Category { get; set; }
    }
}
