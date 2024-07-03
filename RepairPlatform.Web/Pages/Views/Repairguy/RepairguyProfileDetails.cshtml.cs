using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepairPlatform.Entities;
using RepairPlatform.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RepairPlatform.Web.Pages.Views.Repairguy
{
    public class RepairguyProfileDetailsModel : PageModel
    {
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;
        private readonly Repairguy20118046Context _context;
        private readonly RepairguysService _repairguysService;

        public RepairguyProfileDetailsModel(UserManager<AspNetUsers> userManager, SignInManager<AspNetUsers> signInManager, Repairguy20118046Context context, RepairguysService repairguysService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _repairguysService = repairguysService;
        }

        public string? ErrorMessage { get; set; }

        [BindProperty]
        public RepairguyInputModel? Repairguy { get; set; }

        public int RepairguyId { get; set; }

        public Dictionary<string, List<SelectListItem>> GroupOptions { get; set; } = new Dictionary<string, List<SelectListItem>>();

        [BindProperty]
        public List<string> SelectedRepairs { get; set; } = new List<string>();

        public class RepairguyInputModel
        {

            public string? RfirstName { get; set; }

            public string? RlastName { get; set; }


            public string? Rdescription { get; set; }

            [Required]
            public List<string> Repairs { get; set; } = new List<string>();

            [Required]
            public int TownId { get; set; }

            [BindProperty]
            public List<string> SelectedRepairs { get; set; } = new List<string>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Login_Logout/Login");
            }

            var repairguyDto = await _repairguysService.GetByUserId(user.Id);

            if (repairguyDto == null)
            {
                return RedirectToPage("/Login_Logout/Login");
            }

            Repairguy = new RepairguyInputModel
            {
                RfirstName = repairguyDto.RfirstName,
                RlastName = repairguyDto.RlastName,
                Rdescription = repairguyDto.Rdescription,
                Repairs = repairguyDto.Repairs.Select(r => r.RepairId.ToString()).ToList(),
                TownId = repairguyDto.TownId
            };

            RepairguyId = repairguyDto!.RepairguyId;

            var groups = await _context.Groups.Include(g => g.Repairs).ToListAsync();
            foreach (var group in groups)
            {
                GroupOptions[group.CatName] = group.Repairs.Select(r => new SelectListItem
                {
                    Value = r.RepairId.ToString(),
                    Text = r.RepName
                }).ToList();
            }

            SelectedRepairs = repairguyDto.Repairs.Select(r => r.RepairId.ToString()).ToList();

            var towns = await _context.Towns.ToListAsync();
            ViewData["Towns"] = new SelectList(towns, "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Login_Logout/Login");
            }

            var repairguyDto = await _repairguysService.GetByUserId(user.Id);

            if (repairguyDto == null)
            {
                return RedirectToPage("/Login_Logout/Login");
            }

            //var identity = (ClaimsIdentity)User.Identity!;
            //var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            //if (usernameClaim != null)
            //{
            //    identity.RemoveClaim(usernameClaim);
            //    identity.AddClaim(new
            //        (ClaimTypes.Name, user.UserName));
            //}

            //var userClaims = await _userManager.GetClaimsAsync(user);
            //usernameClaim = userClaims.FirstOrDefault(c => c.Type == "username");
            //if (usernameClaim != null)
            //{
            //    await _userManager.RemoveClaimAsync(user, usernameClaim);
            //}
            //await _userManager.AddClaimAsync(user, new Claim("username", user.UserName));

            //await _signInManager.SignOutAsync();
            //await _signInManager.RefreshSignInAsync(user);
            //await _signInManager.SignInAsync(user, isPersistent: false);

            repairguyDto.RfirstName = Repairguy!.RfirstName!;
            repairguyDto.RlastName = Repairguy.RlastName!;
            repairguyDto.Rdescription = Repairguy!.Rdescription!;
            repairguyDto.TownId = Repairguy.TownId;

            var selectedRepairIds = SelectedRepairs.Select(int.Parse).ToList();
            var selectedRepairs = await _context.Repairs.Where(r => selectedRepairIds.Contains(r.RepairId)).ToListAsync();
            repairguyDto.Repairs = selectedRepairs;

            var repairguyDB = await _context.Repairguys.FirstOrDefaultAsync(r => r.RepairguyId == repairguyDto.RepairguyId);
            if (repairguyDB == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid user information.");
                TempData["Message"] = "Неуспешна актуализация на профила. Невалидна информация.";
                TempData["MessageType"] = "error";
                return Page();
            }
            
            if (Repairguy.RfirstName != null) repairguyDB.RfirstName = Repairguy.RfirstName!;
            if (Repairguy.RlastName != null) repairguyDB.RlastName = Repairguy.RlastName!;
            repairguyDB.Rdescription = Repairguy.Rdescription!;
            if (Repairguy.Repairs != null) repairguyDB.Repairs = selectedRepairs;
            if (Repairguy.TownId != 0) repairguyDB.TownId = Repairguy.TownId;

            _context.Repairguys.Update(repairguyDB!);
            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);

            TempData["Message"] = "Успешно актуализирахте своя профил!";
            TempData["MessageType"] = "success";

            return RedirectToPage("/Views/Repairguy/RepairguyProfileDetails");
        }
    }
}
