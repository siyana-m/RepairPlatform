using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services.DTO.Repairguys;
using RepairPlatform.Services;
using RepairPlatform.Entities;
using System.Security.Claims;
using RepairPlatform.Services.DTO.Reviews;
using RepairPlatform.Services.DTO.Reservations;

namespace RepairPlatform.Web.Pages.Views.Client
{
    public class RepairguyProfileModel : PageModel
    {
        private readonly RepairguysService _repairguysService;
        private readonly ReservationsService _reservationsService;
        private readonly ReviewsService _reviewService;
        private readonly ClientsService _clientsService;

        public RepairguyDto? Repairguy { get; set; }
        public bool IsReserved { get; set; }
        public List<Review> Reviews { get; set; }

        [BindProperty]
        public int SelectedReservationId { get; set; }

        [BindProperty]
        public RepairPlatform.Entities.Reservation? Reservation { get; set; }

        [BindProperty]
        public RepairPlatform.Entities.Review? NewReview { get; set; }

        public List<Entities.Reservation> ClientReservations { get; set; }

        public string? Message { get; set; }
        public string? MessageType { get; set; }
        public bool CanLeaveReview { get; set; }
        public double AverageRating { get; set; }

        public RepairguyProfileModel(RepairguysService? repairguysService, ReservationsService? reservationsService, ReviewsService? reviewService, ClientsService? clientsService)
        {
            _repairguysService = repairguysService ?? throw new ArgumentNullException(nameof(repairguysService));
            _reservationsService = reservationsService ?? throw new ArgumentNullException(nameof(reservationsService));
            _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
            _clientsService = clientsService ?? throw new ArgumentNullException(nameof(clientsService));
            Reviews = new List<Review>();
            ClientReservations = new List<Entities.Reservation>();
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Repairguy = await _repairguysService.GetById(id);
            if (Repairguy == null)
            {
                return NotFound();
            }

            var allReviews = await _reviewService.GetReviewsAsync();
            Reviews = allReviews.Where(r => r.RepairguyId == id).ToList();

            if (Reviews.Any())
            {
                AverageRating = Reviews.Average(r => r.Rating);
            }
            else
            {
                AverageRating = 0.0;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clientDto = await _clientsService.GetByUserId(userId!);
            var clientId = clientDto!.ClientId;

            Reservation = await _reservationsService.GetReservationByClientAndRepairguyAsync(clientId, id);
            
            ClientReservations = (await _reservationsService.GetReservationsByClientForRepairguyAsync(clientId, id))
                         .Where(res => !Reviews.Any(r => r.ReservationId == res.ReservationId)).ToList();

            CanLeaveReview = ClientReservations.Any() && !await _reviewService.HasUserReviewedAllReservationsAsync(clientId, ClientReservations);

            foreach (var reservation in ClientReservations)
            {
                if (!await _reviewService.HasUserReviewedReservationAsync(clientId, Reservation!.ReservationId))
                {
                    Reservation = Reservation;
                    NewReview = new Review
                    {
                        RepairguyId = id,
                        ClientId = clientId,
                        GroupId = Reservation.GroupId,
                        RevLocation = Reservation.ResLocation
                    };
                    break;
                }
            }

          
            AverageRating = await _reviewService.GetAverageRatingByRepairguyIdAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostReservationAsync(int repairguyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var clientDto = await _clientsService.GetByUserId(userId!);
            var clientId = clientDto?.ClientId;
            if (clientId == null)
            {
                return BadRequest("Client not found.");
            }
            return RedirectToPage("/Reservation/ReservationForm", new { repairguyId, clientId });
        }


        public async Task<IActionResult> OnPostReviewAsync(int repairguyId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clientDto = await _clientsService.GetByUserId(userId!);
            var clientId = clientDto!.ClientId;
            if (userId == null)
            {
                return Unauthorized();
            }

            var selectedReservation = await _reservationsService.GetReservationById(SelectedReservationId);

            if (await _reviewService.HasUserReviewedReservationAsync(clientId, selectedReservation.ReservationId))
            {
                ModelState.AddModelError(string.Empty, "You have already reviewed this reservation.");
                TempData["Message"] = "Вече сте оставили мнение по всички Ваши резервации към този майстор";
                TempData["MessageType"] = "error";
                return RedirectToPage(new { id = repairguyId });
            }

            NewReview!.RepairguyId = repairguyId;
            NewReview.ReservationId = SelectedReservationId;
            NewReview.RevDateTime = DateTime.Now;
            NewReview.ClientId = clientId;
            NewReview.GroupId = selectedReservation.Group.GroupId;
            await _reviewService.CreateReviewAsync(NewReview);

            TempData["Message"] = "Вашето мнение е успешно добавено.";
            TempData["MessageType"] = "success";

            return RedirectToPage(new { id = repairguyId });
        }

    }
}
