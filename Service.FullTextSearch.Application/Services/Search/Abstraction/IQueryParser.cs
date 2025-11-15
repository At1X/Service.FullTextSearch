using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Application.Services.Search.Abstraction;

public interface IQueryParser
{
    ParsedQuery Parse(string query);
}