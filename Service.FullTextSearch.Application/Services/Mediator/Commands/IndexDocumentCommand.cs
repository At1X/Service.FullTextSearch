using MediatR;
using Service.FullTextSearch.Application.Common.Models;

namespace Service.FullTextSearch.Application.Services.Mediator.Commands;

public record IndexDocumentCommand(string Title, string Content) 
    : IRequest<Result<Guid>>;