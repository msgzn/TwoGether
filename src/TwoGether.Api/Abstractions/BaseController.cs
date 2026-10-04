using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace TwoGether.Api.Abstractions;

[ApiController]
public abstract class BaseController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator { get; } = mediator;
}
