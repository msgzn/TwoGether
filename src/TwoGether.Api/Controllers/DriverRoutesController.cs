using Mediator;
using Microsoft.AspNetCore.Mvc;
using TwoGether.Api.Abstractions;
using TwoGether.Application.Routes.Commands;
using TwoGether.Application.Routes.Queries;

namespace TwoGether.Api.Controllers;

[Route("api/[controller]")]
public class DriverRoutesController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await Mediator.Send(new GetMyDriverRoutesQuery(), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDriverRouteCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));

    [HttpPost("{id:guid}/pause")]
    public async Task<IActionResult> Pause([FromRoute] Guid id, CancellationToken ct)
        => Ok(await Mediator.Send(new SetDriverRouteActiveCommand { Id = id, IsActive = false }, ct));

    [HttpPost("{id:guid}/resume")]
    public async Task<IActionResult> Resume([FromRoute] Guid id, CancellationToken ct)
        => Ok(await Mediator.Send(new SetDriverRouteActiveCommand { Id = id, IsActive = true }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteDriverRouteCommand { Id = id }, ct);
        return NoContent();
    }
}
