using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Infrastructure.Persistence.Repositories;
using Service.FullTextSearch.Infrastructure.Services;
using Service.FullTextSearch.Tests.Services;

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

