using Microsoft.AspNetCore.Mvc;
using RepairPlatform.Services;

namespace RepairPlatform.API.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class RepairguyReservationsController : Controller
    {

        private readonly RepairguysService _repairguysService;

        public RepairguyReservationsController(RepairguysService repairguysService)
        {
            _repairguysService = repairguysService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations(int id)
        {
            var reservations = await _repairguysService.GetRepairguyReservations(id);
            return Ok(reservations);
        }
    }
}
