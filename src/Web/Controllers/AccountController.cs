using Microsoft.AspNetCore.Mvc;

namespace LicenseManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string email, string password, string? returnUrl = null)
    {
        // TODO: Implement actual authentication
        if (email == "admin@example.com" && password == "password")
        {
            _logger.LogInformation("User logged in: {Email}", email);
            
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        TempData["Error"] = "Invalid email or password.";
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        // TODO: Implement actual logout
        _logger.LogInformation("User logged out");
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Profile()
    {
        // TODO: Get actual user profile
        return View();
    }

    [HttpGet]
    public IActionResult Settings()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
