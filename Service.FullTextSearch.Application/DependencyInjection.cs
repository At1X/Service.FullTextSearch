using System.Reflection;
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
using Service.FullTextSearch.Application.SearchResultMapper.Abstraction;
using Service.FullTextSearch.Application.SearchResultMapper.Business;
using Service.FullTextSearch.Application.SearchScorer.Abstraction;
using Service.FullTextSearch.Application.SearchScorer.Business;
using Service.FullTextSearch.Application.StopWordRemover.Abstraction;
using Service.FullTextSearch.Application.TextProcessor.Abstraction;
using Service.FullTextSearch.Application.TextProcessor.Business;
using Service.FullTextSearch.Application.Tokenizer.Abstraction;

namespace Service.FullTextSearch.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // Text Processing
        services.AddSingleton<ITokenizer, Tokenizer.Business.Tokenizer>();
        services.AddSingleton<IStopWordRemover, StopWordRemover.Business.StopWordRemover>();
        services.AddSingleton<ITextProcessor, StandardTextProcessor>();
        services.AddSingleton<IDocumentIndexer, InvertedIndexService>();
    
        // Search Pipeline & Components
        services.AddSingleton<ISearchScoreCalculator, FrequencySumCalculator>();
        services.AddSingleton<ISearchPipeline, InvertedIndexSearchPipeline>();
        services.AddSingleton<IInvertedIndexDocumentUpdater, InvertedIndexDocumentUpdater>();
        services.AddSingleton<IInvertedIndexDocumentRetriever, InvertedIndexDocumentRetriever>();
        services.AddSingleton<ISearchResultBuilder, SearchResultBuilder.Business.SearchResultBuilder>();
    
        // Result Mapping
        services.AddSingleton<IContentSummarizer, ContentSummarizer>();
        services.AddSingleton<ISearchResultMapper, DocumentResultMapper>();
        

        return services;
    }
}