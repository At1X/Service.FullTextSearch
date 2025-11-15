using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.FullTextSearch.Application.Services.Mediator.Queries;

namespace Service.FullTextSearch.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return BadRequest(new { error = "DocumentIndex query cannot be empty" });
        }

        var query = new SearchTermQuery(searchQuery);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data);
    }
}