using Common.Application.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Api.Extensions;
using PaymentService.Application.Payments.Commands.CancelPayment;
using PaymentService.Application.Payments.Commands.CreatePayment;
using PaymentService.Application.Payments.Commands.ProcessPayment;
using PaymentService.Application.Payments.Queries.GetPayment;
using PaymentService.Application.Payments.Queries.GetPayments;
using PaymentService.Contracts.Payments.CancelPayment;
using PaymentService.Contracts.Payments.CreatePayment;
using PaymentService.Contracts.Payments.GetPayment;
using PaymentService.Contracts.Payments.GetPayments;
using PaymentService.Contracts.Payments.ProcessPayment;
using Common.Application.Interfaces.Authentication;

namespace PaymentService.Api.Controllers;

/// <summary>
/// API Controller for managing payments
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly CreatePaymentCommandHandler _createPaymentHandler;
    private readonly ProcessPaymentCommandHandler _processPaymentHandler;
    private readonly CancelPaymentCommandHandler _cancelPaymentHandler;
    private readonly GetPaymentQueryHandler _getPaymentHandler;
    private readonly GetPaymentsQueryHandler _getPaymentsHandler;
    private readonly IUserContext _userContext;

    public PaymentsController(
        CreatePaymentCommandHandler createPaymentHandler,
        ProcessPaymentCommandHandler processPaymentHandler,
        CancelPaymentCommandHandler cancelPaymentHandler,
        GetPaymentQueryHandler getPaymentHandler,
        GetPaymentsQueryHandler getPaymentsHandler,
        IUserContext userContext)
    {
        _createPaymentHandler = createPaymentHandler;
        _processPaymentHandler = processPaymentHandler;
        _cancelPaymentHandler = cancelPaymentHandler;
        _getPaymentHandler = getPaymentHandler;
        _getPaymentsHandler = getPaymentsHandler;
        _userContext = userContext;
    }

    /// <summary>
    /// Create a new payment
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request, CancellationToken ct)
    {
        var tenantId = _userContext.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized("Tenant ID not found in token.");

        var result = await _createPaymentHandler.Handle(request, tenantId.Value, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Process/complete a payment
    /// </summary>
    [HttpPost("{paymentId:guid}/process")]
    public async Task<IActionResult> ProcessPayment(
        [FromRoute] Guid paymentId, 
        [FromBody] ProcessPaymentRequest request, 
        CancellationToken ct)
    {
        var tenantId = _userContext.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized("Tenant ID not found in token.");

        var updated = request with { PaymentId = paymentId };
        var result = await _processPaymentHandler.Handle(updated, tenantId.Value, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Cancel a pending payment
    /// </summary>
    [HttpPost("{paymentId:guid}/cancel")]
    public async Task<IActionResult> CancelPayment([FromRoute] Guid paymentId, CancellationToken ct)
    {
        var tenantId = _userContext.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized("Tenant ID not found in token.");

        var request = new CancelPaymentRequest(paymentId);
        var result = await _cancelPaymentHandler.Handle(request, tenantId.Value, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{paymentId:guid}")]
    public async Task<IActionResult> GetPayment([FromRoute] Guid paymentId, CancellationToken ct)
    {
        var tenantId = _userContext.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized("Tenant ID not found in token.");

        var request = new GetPaymentRequest(paymentId);
        var result = await _getPaymentHandler.Handle(request, tenantId.Value, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get paginated list of payments
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPayments([FromQuery] GetPaymentsRequest request, CancellationToken ct)
    {
        var tenantId = _userContext.TenantId;
        if (!tenantId.HasValue)
            return Unauthorized("Tenant ID not found in token.");

        var result = await _getPaymentsHandler.Handle(request, tenantId.Value, ct);
        return result.ToActionResult(this);
    }
}
