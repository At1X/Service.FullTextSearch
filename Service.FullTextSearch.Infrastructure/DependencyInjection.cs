using System.Reflection;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Domain.Interfaces;
using Service.FullTextSearch.Infrastructure.Persistence.Repositories;
using Service.FullTextSearch.Infrastructure.Services;

namespace Service.FullTextSearch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Text Processing
        services.AddScoped<ITokenizer, Tokenizer>();
        services.AddScoped<IStopWordRemover, StopWordRemover>();
        services.AddScoped<ITextProcessor, StandardTextProcessor>();
    
        // Search Pipeline & Components
        services.AddScoped<ISearchScorer, FrequencyBasedScorer>();
        services.AddScoped<ISearchPipeline, InvertedIndexSearchPipeline>();
    
        // Result Mapping
        services.AddScoped<IContentSummarizer, ContentSummarizer>();
        services.AddScoped<ISearchResultMapper, DocumentResultMapper>();
    
        // Repositories
        services.AddSingleton<IDocumentRepository, InMemoryDocumentRepository>();
        services.AddSingleton<IInvertedIndexRepository, InMemoryInvertedIndexRepository>();

        return services;
    }
}

