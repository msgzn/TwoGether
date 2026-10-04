using Mediator;
using Microsoft.AspNetCore.Mvc;
using TwoGether.Api.Abstractions;
using TwoGether.Application.Users.Commands;
using TwoGether.Application.Users.Queries;

namespace TwoGether.Api.Controllers;

[Route("api/[controller]")]
public class UsersController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
        => Ok(await Mediator.Send(new GetMeQuery(), ct));

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateMyProfileCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));
}
