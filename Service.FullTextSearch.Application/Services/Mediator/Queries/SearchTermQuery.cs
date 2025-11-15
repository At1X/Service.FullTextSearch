using MediatR;
using Service.FullTextSearch.Application.Common.DTOs;
using Service.FullTextSearch.Application.Common.Models;

namespace Service.FullTextSearch.Application.Services.Mediator.Queries;

public record SearchTermQuery(string SearchText) : IRequest<Result<SearchResultDto>>;