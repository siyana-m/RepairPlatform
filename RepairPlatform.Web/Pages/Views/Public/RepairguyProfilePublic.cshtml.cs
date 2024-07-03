using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RepairPlatform.Services.DTO.Repairguys;
using RepairPlatform.Services;
using RepairPlatform.Entities;

namespace RepairPlatform.Web.Pages
{
    public class RepairguyProfilePublicModel : PageModel
    {
        private readonly RepairguysService _repairguysService;

        private readonly ReviewsService _reviewService;


        public RepairguyDto? Repairguy { get; set; }
        public bool IsReserved { get; set; }
        public List<Review> Reviews { get; set; }
        public double AverageRating { get; set; }

        public RepairguyProfilePublicModel(RepairguysService repairguysService, ReviewsService reviewService)
        {
            _repairguysService = repairguysService;
            _reviewService = reviewService;
            Reviews = new List<Review>();
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Repairguy = await _repairguysService.GetById(id);
            if (Repairguy == null)
            {
                return NotFound();
            }

            Reviews = await _reviewService.GetPublicReviewsByRepairguyIdAsync(id);

            if (Reviews.Any())
            {
                AverageRating = Reviews.Average(r => r.Rating);
            }
            else
            {
                AverageRating = 0.0;
            }
            AverageRating = await _reviewService.GetAverageRatingByRepairguyIdAsync(id);


            return Page();
        }

        public IActionResult OnPost(int reservationId)
        {
            return RedirectToPage("/Views/Client/ReservationForm", new { repairGuyId = reservationId });
        }
    }
}
