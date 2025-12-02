using Microsoft.AspNetCore.Mvc;
using LicenseManagement.Web.Services.Abstractions;

namespace LicenseManagement.Web.Controllers;

public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentService paymentService,
        ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, string? status = null)
    {
        var viewModel = await _paymentService.GetPaymentsAsync(page, pageSize, search, status);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var viewModel = await _paymentService.GetPaymentByIdAsync(id);
        if (viewModel == null)
        {
            TempData["Error"] = "Payment not found.";
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refund(Guid id)
    {
        var result = await _paymentService.RefundPaymentAsync(id);
        if (result)
        {
            TempData["Success"] = "Payment refunded successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to refund payment.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }
}
