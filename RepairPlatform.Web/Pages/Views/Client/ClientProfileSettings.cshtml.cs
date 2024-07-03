using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Entities;
using RepairPlatform.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RepairPlatform.Web.Pages.Views.Client
{
    public class ClientProfileSettingsModel : PageModel
    {
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;
        private readonly Repairguy20118046Context _context;
        private readonly ClientsService _clientsService;

        public ClientProfileSettingsModel(UserManager<AspNetUsers> userManager, SignInManager<AspNetUsers> signInManager, Repairguy20118046Context context, ClientsService clientsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _clientsService = clientsService;
        }

        public string? ErrorMessage { get; set; }

        [BindProperty]
        public ClientInputModel? Client { get; set; }

        public int ClientId { get; set; }

        [BindProperty]
        public IFormFile? ProfilePicture { get; set; }

        public class ClientInputModel
        {
            [Required]
            public string? CfirstName { get; set; }
            [Required]
            public string? ClastName { get; set; }
            [Required]
            [EmailAddress]
            public string? Cemail { get; set; }
            [Phone]
            public string? Ctelephone { get; set; }
            public byte[]? Cphoto { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User); 

            if (user == null)
            {
                return RedirectToPage("/Login_Logout/Login");
            }

            var clientDto = await _clientsService.GetByUserId(user.Id);

            if (clientDto == null)
            {
                return RedirectToPage("/Login_Logout/Login");
            }

            Client = new ClientInputModel
            {
                CfirstName = clientDto.CfirstName,
                ClastName = clientDto.ClastName,
                Cemail = clientDto.Cemail,
                Ctelephone = clientDto.Ctelephone,
                Cphoto = clientDto.Cphoto
            };

            ClientId = clientDto!.ClientId;
            return Page();
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
                return RedirectToPage("/Login_Logout/Login");
            }

            var clientDto = await _clientsService.GetByUserId(user.Id); 

            if (clientDto == null)
            {
                return RedirectToPage("/Login_Logout/Login");
            }

            user.UserName = Client!.CfirstName + " " + Client.ClastName;
            var updateResult = await _userManager.UpdateAsync(user);

            var identity = (ClaimsIdentity)User.Identity!;
            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            if (usernameClaim != null)
            {
                identity.RemoveClaim(usernameClaim);
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            }

            // Remove old username claim and add new username claim
            var userClaims = await _userManager.GetClaimsAsync(user);
            usernameClaim = userClaims.FirstOrDefault(c => c.Type == "username");
            if (usernameClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, usernameClaim);
            }
            await _userManager.AddClaimAsync(user, new Claim("username", user.UserName));

            await _signInManager.SignOutAsync();
            await _signInManager.RefreshSignInAsync(user);
            await _signInManager.SignInAsync(user, isPersistent: false);
            var user2 = await _userManager.GetUserAsync(User);

            // Handle file upload
            if (ProfilePicture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ProfilePicture.CopyToAsync(memoryStream);
                    Client!.Cphoto = memoryStream.ToArray();
                }
                clientDto.Cphoto = Client.Cphoto;
            }

            // Update client data
            clientDto.CfirstName = Client!.CfirstName!;
            clientDto.ClastName = Client.ClastName!;
            clientDto.Cemail = Client.Cemail!;
            clientDto.Ctelephone = Client.Ctelephone!;

            var clientDB = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientDto.ClientId);
            if (clientDB == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid user information.");
                TempData["Message"] = "Неуспешна актуализация на профила. Невалидна информация.";
                TempData["MessageType"] = "error";
                return Page();
            }

            clientDB.CfirstName = Client.CfirstName!;
            clientDB.ClastName = Client.ClastName!;
            clientDB.Cemail = Client.Cemail!;
            clientDB.Ctelephone = Client.Ctelephone!;
            clientDB.Cphoto = Client.Cphoto;

            _context.Clients.Update(clientDB!);
            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            TempData["Message"] = "Успешно актуализирахте своя профил!";
            TempData["MessageType"] = "success";

            return RedirectToPage("/Views/Client/ClientProfile");
            //return RedirectToPage();
        }

        //public async Task<IActionResult> OnPostUpdateProfileAsync([FromBody] UserProfile model)
        //{
        //    var user = await _userManager.GetUserAsync(User); // Retrieve the current user

        //    if (user == null)
        //    {
        //        return new JsonResult(new { success = false });
        //    }

        //    var clientDto = await _clientsService.GetByUserId(user.Id); // Assign the user's ID to ClientId

        //    if (clientDto == null)
        //    {
        //        return new JsonResult(new { success = false });
        //    }

        //    // Update client data
        //    clientDto.CfirstName = model.FirstName!;
        //    clientDto.ClastName = model.LastName!;

        //    var clientDB = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientDto.ClientId);
        //    if (clientDB == null)
        //    {
        //        return new JsonResult(new { success = false });
        //    }

        //    clientDB.CfirstName = model.FirstName!;
        //    clientDB.ClastName = model.LastName!;

        //    _context.Clients.Update(clientDB!);
        //    await _context.SaveChangesAsync();
        //    return new JsonResult(new { success = true });
        //}



        //public class UserProfile
        //{
        //    public string? FirstName { get; set; }
        //    public string? LastName { get; set; }
        //}
    }
}
