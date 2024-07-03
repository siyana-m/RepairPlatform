using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Entities;
using RepairPlatform.Services.DTO.Clients;
using RepairPlatform.Services.DTO.Reservations;
using RepairPlatform.Services;

namespace RepairPlatform.Web.Pages.Views.Client
{
    public class ClientHistoryReservationsModel : PageModel
    {
        private readonly ReservationsService _reservationsService;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly ClientsService _clientsService;

        public ClientHistoryReservationsModel(UserManager<AspNetUsers> userManager, ReservationsService reservationsService, ClientsService clientsService)
        {
            _reservationsService = reservationsService;
            _userManager = userManager;
            _clientsService = clientsService;
        }

        public List<ReservationDto>? Reservations { get; set; }
        public ClientDto? Client { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadReservationsAsync().ConfigureAwait(false);
            return Page();
        }

        private async Task LoadReservationsAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null) return;

            var clientDto = await _clientsService.GetByUserId(user.Id).ConfigureAwait(false);
            if (clientDto == null) return;

            Reservations =new List<ReservationDto>();
            if (clientDto.Reservations != null && clientDto.Reservations.Any())
            {
                foreach (var reservation in clientDto.Reservations)
                {
                    Reservations.Add(new ReservationDto
                    {
                        ReservationId = reservation.ReservationId,
                        ResName = reservation.ResName,
                        Group = reservation.Group,
                        ResDateTime = reservation.ResDateTime,
                        ResLocation = reservation.ResLocation,
                        ResComment = reservation.ResComment,
                        ResStatus = reservation.ResStatus,
                        Repairguy = reservation.Repairguy, 
                    });
                }
            }
        }

        public string GetStatusClass(string status)
        {
            return status switch
            {
                "Приета" => "status-accepted",
                "Отхвърлена" => "status-declined",
                _ => "status-pending",
            };
        }

        public async Task<IActionResult> OnPostDeleteAsync(int reservationId)
        {
            var success = await _reservationsService.DeleteReservationByClientAsync(reservationId).ConfigureAwait(false);
            if (!success)   
            {

                return NotFound();
            }


            await LoadReservationsAsync().ConfigureAwait(false);
            return RedirectToPage();
        }
    }
}
