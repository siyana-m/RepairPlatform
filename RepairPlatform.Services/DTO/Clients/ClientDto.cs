using RepairPlatform.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Clients
{
    public class ClientDto
    {
        public int ClientId { get; set; }

        public string CfirstName { get; set; } = null!;

        public string ClastName { get; set; } = null!;

        public string Ctelephone { get; set; } = null!;

        public string Cemail { get; set; } = null!;

        public string Cpassword { get; set; } = null!;

        public byte[]? Cphoto { get; set; }

        public string? Cstatus { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
