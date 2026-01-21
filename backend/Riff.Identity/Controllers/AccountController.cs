using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Riff.Identity.Models;
using Riff.Infrastructure.Entities;
using Duende.IdentityServer.Services;

namespace Riff.Identity.Controllers;

public class AccountController(
    SignInManager<User> signInManager,
    UserManager<User> userManager,
    IIdentityServerInteractionService interaction)
    : Controller
{
    private readonly UserManager<User> _userManager = userManager;

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        var model = new LoginViewModel { ReturnUrl = returnUrl };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                
                return Redirect("~/");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        await signInManager.SignOutAsync();
        
        var logoutRequest = await interaction.GetLogoutContextAsync(logoutId);

        if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
        {
            return Redirect("~/");
        }

        return Redirect(logoutRequest.PostLogoutRedirectUri);
    }
}