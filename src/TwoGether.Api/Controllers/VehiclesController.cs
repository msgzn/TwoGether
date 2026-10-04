using Mediator;
using Microsoft.AspNetCore.Mvc;
using TwoGether.Api.Abstractions;
using TwoGether.Application.Vehicles.Commands;
using TwoGether.Application.Vehicles.Queries;

namespace TwoGether.Api.Controllers;

[Route("api/[controller]")]
public class VehiclesController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken ct)
        => Ok(await Mediator.Send(new GetMyVehiclesQuery(), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVehicleCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateVehicleCommand command, CancellationToken ct)
    {
        command.Id = id;
        return Ok(await Mediator.Send(command, ct));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteVehicleCommand { Id = id }, ct);
        return NoContent();
    }
}
