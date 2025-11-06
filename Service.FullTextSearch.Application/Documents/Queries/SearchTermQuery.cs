using MediatR;
using Service.FullTextSearch.Application.Common.Models;
using Service.FullTextSearch.Application.Documents.DTOs;

namespace Service.FullTextSearch.Application.Documents.Queries;

public record SearchTermQuery(string SearchText) : IRequest<Result<SearchResultDto>>;