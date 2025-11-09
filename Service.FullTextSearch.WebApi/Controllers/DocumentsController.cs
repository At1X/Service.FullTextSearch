using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.FullTextSearch.Application.Common.DTOs;
using Service.FullTextSearch.Application.MedatorActions.Commands;

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
        var indexDocumentCommand = new IndexDocumentCommand(
            request.Title, 
            request.Content);

        var result = await _mediator.Send(indexDocumentCommand);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { documentId = result.Data, message = "Document indexed successfully" });
    }
    
}