using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Entities;
using RepairPlatform.Services;
using System.ComponentModel.DataAnnotations;

namespace RepairPlatform.Web.Pages.Views.Repairguy
{
    public class RepairguyProfileChangePasswordModel : PageModel
    {
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;

        public RepairguyProfileChangePasswordModel(UserManager<AspNetUsers> userManager, SignInManager<AspNetUsers> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public string? ErrorMessage { get; set; }

        [BindProperty]
        public RepairguyInputModel? Input { get; set; }
        public class RepairguyInputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Old Password")]
            public string? OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string? NewPassword { get; set; }
            
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm New Password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["Message"] = "Your password has been changed.";

            return RedirectToPage("/Views/Repairguy/RepairguyProfileChangePassword");
        }
    }
}
