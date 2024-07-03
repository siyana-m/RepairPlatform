using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services.DTO.Reservations;
using RepairPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using RepairPlatform.Entities;

namespace RepairPlatform.Web.Pages.Reservation
{
    [Authorize]
    public class SuccessfulReservationModel : PageModel
    {
        private readonly ReservationsService _reservationService;
        private readonly UserManager<AspNetUsers> _userManager;

        [BindProperty]
        public ReservationDto? Reservation { get; set; }

        public SuccessfulReservationModel(ReservationsService reservationService, UserManager<AspNetUsers> userManager)
        {
            _reservationService = reservationService;
            _userManager = userManager;
        }

        public async Task OnGetAsync(int id)
        {
            Reservation = await _reservationService.GetReservationById(id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var reservation = await _reservationService.ConfirmReservation(Reservation!.ReservationId);
            return RedirectToPage("/Views/Client/ClientView", new {id = reservation.ClientId });
        }
    }
}
