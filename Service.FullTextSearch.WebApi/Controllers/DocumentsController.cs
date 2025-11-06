using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.FullTextSearch.Application.Documents.Commands;
using Service.FullTextSearch.Application.Documents.DTOs;

namespace Service.FullTextSearch.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> IndexDocument([FromBody] IndexDocumentDto request)
    {
        var command = new IndexDocumentCommand(
            request.Title, 
            request.Content, 
            request.FilePath ?? string.Empty);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { documentId = result.Data, message = "Document indexed successfully" });
    }
    
}