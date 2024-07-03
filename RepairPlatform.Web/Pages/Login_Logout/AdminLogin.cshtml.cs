using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Entities;
using RepairPlatform.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RepairPlatform.Web.Pages.Login_Logout
{
    public class AdminLoginModel : PageModel
    {
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;

        public string? ErrorMessage { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        [BindProperty]
        public string? ReturnUrl { get; set; }

        public AdminLoginModel(UserManager<AspNetUsers> userManager, SignInManager<AspNetUsers> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        public void OnGet(string returnUrl = null!)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.FindByEmailAsync(Email!);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                ErrorMessage = "Грешен потребител.";
                return Page();
            }

            var logResult = await _signInManager.PasswordSignInAsync(user, Password!, isPersistent: false, lockoutOnFailure: false);
            if (!logResult.Succeeded)
            {
                ErrorMessage = "Грешна парола.";
                return Page();
            }


            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("admin"))
            {
                

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id),
                    new (ClaimTypes.Name, user.Email),
                    new(ClaimTypes.Email, user.Email),

                    new("UserType", "admin")

                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);


                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }

                return RedirectToPage("/Views/Admin/AdminView", new { id = user.Id });
            }

            else
            {
                ErrorMessage = "Грешна роля.";
                return Page();
            }
        }
    }
}
