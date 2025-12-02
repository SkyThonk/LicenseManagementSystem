using Common.Application.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.Extensions;
using NotificationService.Contracts.Templates.CreateTemplate;
using NotificationService.Contracts.Templates.GetTemplates;
using NotificationService.Contracts.Templates.UpdateTemplate;
using Wolverine;

namespace NotificationService.Api.Controllers;

/// <summary>
/// API Controller for managing notification templates
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TemplatesController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public TemplatesController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Create a new notification template
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<CreateTemplateResponse>>(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Update an existing notification template
    /// </summary>
    [HttpPut("{templateId:guid}")]
    public async Task<IActionResult> UpdateTemplate([FromRoute] Guid templateId, [FromBody] UpdateTemplateRequest request, CancellationToken ct)
    {
        var updated = request with { TemplateId = templateId };
        var result = await _messageBus.InvokeAsync<Result<UpdateTemplateResponse>>(updated, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Get all notification templates for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTemplates([FromQuery] GetTemplatesRequest request, CancellationToken ct)
    {
        var result = await _messageBus.InvokeAsync<Result<GetTemplatesResponse>>(request, ct);
        return result.ToActionResult(this);
    }
}
