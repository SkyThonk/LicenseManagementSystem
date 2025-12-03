using LicenseManagement.Web.Models.Auth;
using LicenseManagement.Web.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthService authService, ILogger<AccountController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // If already authenticated, redirect to dashboard
        if (_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Dashboard");
        }

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, errorMessage, response) = await _authService.LoginAsync(model.Email, model.Password);

        if (success && response != null)
        {
            _logger.LogInformation("User logged in: {Email}", model.Email);
            
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        model.ErrorMessage = errorMessage ?? "Invalid email or password.";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        var user = _authService.GetCurrentUser();
        _authService.ClearUserSession();
        _logger.LogInformation("User logged out: {Email}", user?.Email ?? "Unknown");
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Profile()
    {
        var user = _authService.GetCurrentUser();
        if (user == null)
        {
            return RedirectToAction(nameof(Login));
        }
        
        ViewBag.User = user;
        return View();
    }

    [HttpGet]
    public IActionResult Settings()
    {
        if (!_authService.IsAuthenticated())
        {
            return RedirectToAction(nameof(Login));
        }
        return View();
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
