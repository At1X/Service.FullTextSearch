using MediatR;
using Service.FullTextSearch.Application.Common.Models;

namespace Service.FullTextSearch.Application.Documents.Commands;

public record IndexDocumentCommand(string Title, string Content, string FilePath = "") 
    : IRequest<Result<Guid>>;