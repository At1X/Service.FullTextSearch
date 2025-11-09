using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.ContentSummerizer.Abstraction;
using Service.FullTextSearch.Application.ContentSummerizer.Business;
using Service.FullTextSearch.Application.DocumentIndexer.Abstraction;
using Service.FullTextSearch.Application.DocumentIndexer.Business;
using Service.FullTextSearch.Application.InvertedIndexDocumentActions.Abstraction;
using Service.FullTextSearch.Application.InvertedIndexDocumentActions.Business;
using Service.FullTextSearch.Application.SearchQueryPipeline.Abstraction;
using Service.FullTextSearch.Application.SearchQueryPipeline.Business;
using Service.FullTextSearch.Application.SearchResultBuilder.Abstraction;
using Service.FullTextSearch.Application.SearchResultBuilder.Business;
using Service.FullTextSearch.Application.SearchResultMapper.Abstraction;
using Service.FullTextSearch.Application.SearchResultMapper.Business;
using Service.FullTextSearch.Application.SearchScorer.Abstraction;
using Service.FullTextSearch.Application.SearchScorer.Business;
using Service.FullTextSearch.Application.StopWordRemover.Abstraction;
using Service.FullTextSearch.Application.StopWordRemover.Business;
using Service.FullTextSearch.Application.TextProcessor.Abstraction;
using Service.FullTextSearch.Application.TextProcessor.Business;
using Service.FullTextSearch.Application.Tokenizer.Abstraction;
using Service.FullTextSearch.Application.Tokenizer.Business;
using Service.FullTextSearch.Infrastructure.Persistence.Repositories;

namespace Service.FullTextSearch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Text Processing
        services.AddScoped<ITokenizer, Tokenizer>();
        services.AddScoped<IStopWordRemover, StopWordRemover>();
        services.AddScoped<ITextProcessor, StandardTextProcessor>();
        services.AddScoped<IDocumentIndexer, InvertedIndexService>();
    
        // Search Pipeline & Components
        services.AddScoped<ISearchScorer, FrequencyBasedScorer>();
        services.AddScoped<ISearchPipeline, InvertedIndexSearchPipeline>();
        services.AddScoped<IInvertedIndexDocumentUpdater, InvertedIndexDocumentUpdater>();
        services.AddScoped<IInvertedIndexDocumentRetriever, InvertedIndexDocumentRetriever>();
        services.AddScoped<ISearchResultBuilder, SearchResultBuilder>();
    
        // Result Mapping
        services.AddScoped<IContentSummarizer, ContentSummarizer>();
        services.AddScoped<ISearchResultMapper, DocumentResultMapper>();
    
        // Repositories
        services.AddSingleton<IDocumentRepository, InMemoryDocumentRepository>();
        services.AddSingleton<IInvertedIndexRepository, InMemoryInvertedIndexRepository>();

        return services;
    }
}

