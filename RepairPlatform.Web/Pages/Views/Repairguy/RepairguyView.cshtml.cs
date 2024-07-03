using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services.DTO.Repairguys;
using RepairPlatform.Services;
using RepairPlatform.Services.DTO.Reservations;
using RepairPlatform.Services.DTO.Reviews;

namespace RepairPlatform.Web.Pages.Views.Repairguy
{
    public class RepairguyViewModel : PageModel
    {
        private readonly RepairguysService _repairguysService;
        private readonly ReservationsService _reservationsService;
        private readonly ReviewsService _reviewService;


        public RepairguyDto? Repairguy { get; set; }
        public List<ReservationDto>? Reservations { get; set; } = new List<ReservationDto>();
        public Dictionary<string, List<string>>? GroupedRepairs { get; set; } = new Dictionary<string, List<string>>();
        public double AverageRating { get; set; }

        public RepairguyViewModel(RepairguysService repairguysService, ReservationsService reservationsService, ReviewsService reviewService)
        {
            _repairguysService = repairguysService;
            _reservationsService = reservationsService;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Repairguy = await _repairguysService.GetById(id);
            if (Repairguy == null)
            {
                return NotFound();
            }

            if (Repairguy.Repairs != null && Repairguy.Repairs.Any())
            {
                GroupedRepairs = Repairguy.Repairs
                    .SelectMany(r => r.Groups.Select(g => new { g.CatName, r.RepName }))
                    .GroupBy(x => x.CatName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.RepName).ToList());
            }

            Reservations = await _reservationsService.GetReservationsByRepairguyIdAsync(id);

            var reviews = await _reviewService.GetReviewsByRepairguyIdAsync(id);
            if (reviews.Any())
            {
                AverageRating = reviews.Average(r => r.Rating);
            }
            else
            {
                AverageRating = 0.0;
            }

            return Page();
        }
    }
}
