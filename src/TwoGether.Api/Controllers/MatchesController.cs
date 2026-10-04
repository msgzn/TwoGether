using Mediator;
using Microsoft.AspNetCore.Mvc;
using TwoGether.Api.Abstractions;
using TwoGether.Application.Matches.Queries;

namespace TwoGether.Api.Controllers;

[Route("api/[controller]")]
public class MatchesController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await Mediator.Send(new GetMyMatchesQuery(), ct));
}
