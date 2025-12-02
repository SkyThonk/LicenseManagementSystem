using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Payments.Commands.CancelPayment;
using PaymentService.Application.Payments.Commands.CreatePayment;
using PaymentService.Application.Payments.Commands.ProcessPayment;
using PaymentService.Application.Payments.Queries.GetPayment;
using PaymentService.Application.Payments.Queries.GetPayments;

namespace PaymentService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register command handlers
        services.AddScoped<CreatePaymentCommandHandler>();
        services.AddScoped<ProcessPaymentCommandHandler>();
        services.AddScoped<CancelPaymentCommandHandler>();

        // Register query handlers
        services.AddScoped<GetPaymentQueryHandler>();
        services.AddScoped<GetPaymentsQueryHandler>();

        return services;
    }
}
