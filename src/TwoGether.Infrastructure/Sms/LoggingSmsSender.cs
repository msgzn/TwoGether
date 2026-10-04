using Microsoft.Extensions.Logging;
using TwoGether.Application.Abstractions;
using TwoGether.Domain.Users;

namespace TwoGether.Infrastructure.Sms;

internal sealed class LoggingSmsSender(ILogger<LoggingSmsSender> logger) : ISmsSender
{
    public Task SendAsync(PhoneNumber phoneNumber, string message, CancellationToken ct)
    {
        logger.LogWarning("SMS is not sent (mock). To {PhoneNumber}: {Message}", phoneNumber.Value, message);
        return Task.CompletedTask;
    }
}
