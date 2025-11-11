using System.Reflection;
using Service.FullTextSearch.Application.Services.Indexing.Abstraction;
using Service.FullTextSearch.Application.Services.Indexing.Business;
using Service.FullTextSearch.Application.Services.Search.Abstraction;
using Service.FullTextSearch.Application.Services.Search.Business;
using Service.FullTextSearch.Application.Services.TextProcessing.Abstraction;
using Service.FullTextSearch.Application.Services.TextProcessing.Business;
using Service.FullTextSearch.Application.Services.Tokenization.Abstraction;
using Service.FullTextSearch.Application.Services.Tokenization.Business;

namespace Service.FullTextSearch.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Mediator
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // Text Processing
        services.AddSingleton<IStopWordRemover, StopWordRemover>();
        services.AddSingleton<ITextProcessor, StandardTextProcessor>();
        services.AddSingleton<IContentSummarizer, ContentSummarizer>();
        
        // Indexing
        services.AddSingleton<IDocumentIndexer, InvertedIndexService>();
        services.AddSingleton<IInvertedIndexDocumentUpdater, InvertedIndexDocumentUpdater>();
        services.AddSingleton<IInvertedIndexDocumentRetriever, InvertedIndexDocumentRetriever>();
    
        // Search
        services.AddSingleton<ISearchResultMapper, DocumentResultMapper>();
        services.AddSingleton<ISearchFilterAggregator, SearchFilterAggregator>();
        services.AddSingleton<ISearchScoreCalculator, FrequencySumCalculator>();
        services.AddSingleton<ISearchPipeline, AdvancedInvertedIndexSearchPipeline>();
        services.AddSingleton<ISearchResultBuilder, SearchResultBuilder>();
        services.AddSingleton<IQueryParser, AdvancedQueryParser>();
        
        // Tokenization
        services.AddSingleton<ITokenizer, Tokenizer>();
        services.AddSingleton<ITokenClassifiedAggregator, TokenClassifiedAggregator>();
        services.AddSingleton<ITokenHandler, ExcludedTokenHandler>();
        services.AddSingleton<ITokenHandler, RequiredTokenHandler>();
        services.AddSingleton<ITokenHandler, OptionalTokenHandler>();
        services.AddSingleton<ITokenClassifier, DefaultTokenClassifier>();
        
        return services;
    }
}