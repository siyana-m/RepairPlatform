using RepairPlatform.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Reservations
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }

        public int ClientId { get; set; }

        public int RepairguyId { get; set; }

        public int GroupId { get; set; }

        public string ResName { get; set; } = null!;

        public DateTime ResDateTime { get; set; }

        public string ResLocation { get; set; } = null!;

        public string? ResComment { get; set; }

        public string? ResStatus { get; set; }

        public virtual Client Client { get; set; } = null!;

        public virtual Group Group { get; set; } = null!;

        public virtual Repairguy Repairguy { get; set; } = null!;

    }
}
