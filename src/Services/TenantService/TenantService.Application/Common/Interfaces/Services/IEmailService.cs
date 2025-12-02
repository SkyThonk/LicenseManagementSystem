namespace TenantService.Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string resetToken, string firstName, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default);
}
