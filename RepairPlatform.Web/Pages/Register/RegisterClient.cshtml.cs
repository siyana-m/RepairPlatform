using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services.DTO.Repairguys;
using RepairPlatform.Services;
using RepairPlatform.Services.DTO.Clients;

namespace RepairPlatform.Web.Pages.Register
{
    public class RegisterClientModel : PageModel
    {
        private readonly ClientsService _clientsService;

        [BindProperty]
        public CreateClientDto? Client { get; set; }

        public RegisterClientModel(ClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var existingClient = await _clientsService.GetByEmailOrPhone(Client!.CEmail!, Client.CTelephone!);
            if (existingClient != null)
            {
                ModelState.AddModelError(string.Empty, "Клиент с този имейл или телефонен номер вече съществува.");
                return Page();
            }
            try
            {
                await _clientsService.Create(Client!);
                return RedirectToPage("/Register/RegistrationSuccess"); 
            }
            catch
            {
                return RedirectToPage("/Register/RegistrationFail"); 
            }
        }
    }
}
