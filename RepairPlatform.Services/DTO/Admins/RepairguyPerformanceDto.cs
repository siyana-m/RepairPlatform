using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Admins
{
    public class RepairguyPerformanceDto
    {
        public int RepairguyId { get; set; }
        public string? RfirstName { get; set; }
        public string? RlastName { get; set; }
        public string? Remail { get; set; }
        public string? Rstatus { get; set; }
        public double AverageRating { get; set; }
        public int TotalReservations { get; set; }
        public int CompletedReservations { get; set; }
    }
}
