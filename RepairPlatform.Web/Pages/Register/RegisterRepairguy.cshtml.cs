using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services.DTO.Repairguys;
using RepairPlatform.Services;
using RepairPlatform.Entities;
using Microsoft.EntityFrameworkCore;

namespace RepairPlatform.Web.Pages.Register
{
    public class RegisterRepairguyModel : PageModel
    {
        private readonly RepairguysService _repairguysService;
        private readonly Repairguy20118046Context _dbContext;

        [BindProperty]
        public CreateRepairguyDto? Repairguy { get; set; }

        public List<Town> Towns { get; set; } = new List<Town>();

        public RegisterRepairguyModel(RepairguysService repairguysService, Repairguy20118046Context dbContext)
        {
            _repairguysService = repairguysService;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnGetAsync()
        {
         
            Towns = await _dbContext.Towns.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Towns = await _dbContext.Towns.ToListAsync();
                return Page();
            }
            var existingRepairguy = await _repairguysService.GetByEmail(Repairguy!.Remail);
            if (existingRepairguy != null)
            {
                ModelState.AddModelError(string.Empty, "A repairguy with this email already exists.");
                TempData["Message"] = "Майстор с такъв имейл вече съществува.";
                TempData["MessageType"] = "error";
                Towns = await _dbContext.Towns.ToListAsync();
                return Page();
            }
            try
            {
                var result = await _repairguysService.Create(Repairguy!);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    TempData["Message"] = "Error occurred during registration.";
                    TempData["MessageType"] = "error";
                    Towns = await _dbContext.Towns.ToListAsync();
                    return Page();
                }
                return RedirectToPage("/Register/RegistrationSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "error";
                Towns = await _dbContext.Towns.ToListAsync();
                return Page();
               // return RedirectToPage("/Register/RegistrationFail");
            }
        }
    }
}
