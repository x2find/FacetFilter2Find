FacetFilter2Find
===========

Adds facet filtering to EPiServer Find's .NET API

### Build

In order to build FacetFilter2Find using NuGet < 2.7 the NuGet packages that it depends on must be restored.
See http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages

### Usage

To filter a facet to be calculated on only a subset of the documents in the result set:

```c#
result = client.Search<Document>()
                        .TermsFacetFor(x => x.Category, x => x.Type.Match("pdf"))
                        .GetResult();

facet = result.TermsFacetFor(x => x.Category);
```

To add multiple facets on a single field with different filters:

```c#
result = client.Search<Document>()
                        .TermsFacetFor(x => x.Category, x => x.Type.Match("pdf"), x => x.Name = "PdfCategories")
                        .TermsFacetFor(x => x.Category, x => x.Type.Match("doc"), x => x.Name = "DocCategories")
                        .GetResult();

pdfTypeFacet = result.Facets["PdfCategories"] as TermsFacet;
docTypeFacet = result.Facets["DocCategories"] as TermsFacet;
```
