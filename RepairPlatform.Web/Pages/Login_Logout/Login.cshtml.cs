using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Identity;
using RepairPlatform.Entities;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations;

namespace RepairPlatform.Web.Pages.Login_Logout
{
    public class LoginModel : PageModel
    {
        private readonly RepairguysService _repairguysService;
        private readonly ClientsService _clientsService;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;

        public string? ErrorMessage { get; set; }

        [Required]
        [BindProperty]
        public string? UserType { get; set; }

        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        [BindProperty]
        public string? ReturnUrl { get; set; }

        public LoginModel(RepairguysService repairguysService, ClientsService clientsService, UserManager<AspNetUsers> userManager, SignInManager<AspNetUsers> signInManager)
        {
            _repairguysService = repairguysService;
            _clientsService = clientsService;
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

            if (!ModelState.ContainsKey("UserType"))
            {
                ModelState.AddModelError(string.Empty, "Please, choose your role.");
                ErrorMessage = "Моля, изберете вашата роля.";
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

            if ((UserType == "Repairguy") && (logResult.Succeeded))
            {
                var repairguy = await _repairguysService.AuthenticateRepairguyAsync(Email!, user.Id);
                if (repairguy == null)
                {
                    ErrorMessage = "Липсва майстор с посочените данни.";
                    return Page();
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, repairguy.RfirstName + " " + repairguy.RlastName),
                    new(ClaimTypes.Email, repairguy.Remail),
                    new(ClaimTypes.NameIdentifier, user.Id),
                    new("UserType", "Repairguy")
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                //string photoBase64;
                //if (repairguy.Rphoto != null && repairguy.Rphoto.Length > 0)
                //{
                //    photoBase64 = $"data:image/png;base64,{ImageHelper.ToBase64String(repairguy.Rphoto)}";
                //}
                //else
                //{
                //    photoBase64 = Url.Content("~/profile-circle-icon-512x512-zxne30hp.png");
                //}
                //claims.Add(new Claim("ProfileImageUrl", photoBase64));

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

                return RedirectToPage("/Views/Repairguy/RepairguyView", new { id = repairguy.RepairguyId });
            }
            
            
            else if ((UserType == "Client") && (logResult.Succeeded))
            {
                var client = await _clientsService.AuthenticateClientAsync(Email!, user.Id);
                if (client == null)
                {
                    ErrorMessage = "Липсва клиент с посочените данни.";
                    return Page();
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, client.CfirstName + " " + client.ClastName),
                    new(ClaimTypes.Email, user.Email!),
                    new (ClaimTypes.NameIdentifier, user.Id),
                    new("UserType", "Client")
                };
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                //string photoBase64;
                //if (client.Cphoto != null && client.Cphoto.Length > 0)
                //{
                //    photoBase64 = $"data:image/png;base64,{ImageHelper.ToBase64String(client.Cphoto)}";
                //}
                //else
                //{
                //    photoBase64 = Url.Content("~/profile-circle-icon-512x512-zxne30hp.png");
                //}
                //claims.Add(new Claim("ProfileImageUrl", photoBase64));


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


                return RedirectToPage("/Views/Client/ClientView", new { id = client.ClientId });
            }
            else
            {
                ErrorMessage = "Грешна роля.";
                return Page();
            }
        }
    }
}
