using Microsoft.Extensions.Logging;
using TenantService.Application.Common.Interfaces.Services;

namespace TenantService.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken, string firstName, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask; // Simulate async operation

        // For now, log to console. Replace with actual email sending logic
        _logger.LogInformation(
            @"
================================================================================
PASSWORD RESET EMAIL
================================================================================
To: {Email}
Subject: Password Reset Request

Hello {FirstName},

We received a request to reset your password. Please use the following token to reset your password:

Reset Token: {ResetToken}

This token will expire in 1 hour.

If you didn't request this password reset, please ignore this email.

Best regards,
License Management System
================================================================================
",
            email, firstName, resetToken);

        // TODO: Implement actual email sending using SMTP or email service provider
        // Example using SMTP:
        // using var smtpClient = new SmtpClient(smtpHost, smtpPort);
        // smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
        // var mailMessage = new MailMessage(...);
        // await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask; // Simulate async operation

        _logger.LogInformation(
                   @"
================================================================================
WELCOME EMAIL
================================================================================
To: {Email}
Subject: Welcome to License Management System

Hello {FirstName},

Welcome to the License Management System! Your account has been successfully created.

You can now log in using your email address.

Best regards,
License Management System
================================================================================
",
                   email, firstName);

        // TODO: Implement actual email sending
    }
}
