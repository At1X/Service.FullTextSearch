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
        services.AddSingleton<IDocumentRepository, InMemoryDocumentRepository>();
        services.AddSingleton<IInvertedIndexRepository, InMemoryInvertedIndexRepository>();

        services.AddScoped<ITokenizer, Tokenizer>();
        services.AddScoped<IStopWordRemover, StopWordRemover>();

        return services;
    }
}

