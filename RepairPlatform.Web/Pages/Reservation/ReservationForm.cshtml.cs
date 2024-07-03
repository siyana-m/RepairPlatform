using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services.DTO.Reservations;
using RepairPlatform.Services;
using RepairPlatform.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using RepairPlatform.Services.DTO.Groups;
using System.Text.RegularExpressions;
using System;
using Microsoft.AspNetCore.Identity;

namespace RepairPlatform.Web.Pages.Reservation

{
    [Authorize]
    public class ReservationFormModel : PageModel
    {
        private readonly ReservationsService _reservationService;
        private readonly Repairguy20118046Context _dbContext;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly ClientsService _clientsService;

        [BindProperty]
        public ReservationDto? Reservation { get; set; }

        public List<GroupDto>? Groups { get; set; }

        public int RepairGuyId { get; set; }

        public int ClientId { get; set; }

        public ReservationFormModel(ReservationsService reservationService, Repairguy20118046Context dbContext, UserManager<AspNetUsers> userManager, ClientsService clientsService)
        {
            _reservationService = reservationService;
            _dbContext = dbContext;
            _userManager = userManager;
            _clientsService = clientsService;
        }

        public async Task OnGetAsync(int repairGuyId, int clientId)
        {
            RepairGuyId = repairGuyId;
            var user = await _userManager.GetUserAsync(User);
            var clientDto = await _clientsService.GetByUserId(user!.Id);
           
            Groups = await _dbContext.Repairguys
                .Where(rg => rg.RepairguyId == repairGuyId)
                .SelectMany(rg => rg.Repairs)
                .SelectMany(r => r.Groups)
                 .Select(g => new GroupDto
                 {
                     CatName = g.CatName,
                     GroupId = g.GroupId

                 })
                .Distinct()
                .ToListAsync();

            Reservation = new ReservationDto { RepairguyId = repairGuyId, ClientId = clientDto!.ClientId };
            ClientId = clientDto!.ClientId;
        }

        public async Task<IActionResult> OnPostAsync(string action, int repairGuyId)
        {
            if (action == "continue")
            {
               var reservationId = await _reservationService.CreateReservation(Reservation!);

                if (reservationId > 0)
                {
                    return RedirectToPage("/Reservation/SuccessfulReservation", new { id = reservationId });
                }
                return RedirectToPage("/Reservation/FailedReservation");
            }

            return RedirectToPage(new { id = repairGuyId });
        }
    }
}
