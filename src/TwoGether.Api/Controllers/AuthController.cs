using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TwoGether.Api.Abstractions;
using TwoGether.Api.Infrastructure;
using TwoGether.Application.Auth.Commands;

namespace TwoGether.Api.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost("request-code")]
    [EnableRateLimiting(AuthenticationSetup.SmsRateLimitPolicy)]
    public async Task<IActionResult> RequestCode([FromBody] RequestCodeCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
        => Ok(await Mediator.Send(command, ct));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken ct)
    {
        await Mediator.Send(command, ct);
        return NoContent();
    }
}
