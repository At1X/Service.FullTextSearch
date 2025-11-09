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
        services.AddSingleton<ITokenizer, Tokenizer>();
        services.AddSingleton<IStopWordRemover, StopWordRemover>();
        services.AddSingleton<ITextProcessor, StandardTextProcessor>();
        services.AddSingleton<IDocumentIndexer, InvertedIndexService>();
    
        // Search Pipeline & Components
        services.AddSingleton<ISearchScoreCalculator, FrequencySumCalculator>();
        services.AddSingleton<ISearchPipeline, InvertedIndexSearchPipeline>();
        services.AddSingleton<IInvertedIndexDocumentUpdater, InvertedIndexDocumentUpdater>();
        services.AddSingleton<IInvertedIndexDocumentRetriever, InvertedIndexDocumentRetriever>();
        services.AddSingleton<ISearchResultBuilder, SearchResultBuilder>();
    
        // Result Mapping
        services.AddSingleton<IContentSummarizer, ContentSummarizer>();
        services.AddSingleton<ISearchResultMapper, DocumentResultMapper>();
    
        // Repositories
        services.AddSingleton<IDocumentRepository, InMemoryDocumentRepository>();
        services.AddSingleton<IInvertedIndexRepository, InMemoryInvertedIndexRepository>();

        return services;
    }
}

