using RepairPlatform.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Repairguys
{
    public class RepairguyDto
    {
        public int RepairguyId { get; set; }

        public string RfirstName { get; set; } = null!;

        public string RlastName { get; set; } = null!;

        public string Rtelephone { get; set; } = null!;

        public string Remail { get; set; } = null!;

        public string Rpassword { get; set; } = null!;

        public string? Rdescription { get; set; }

        public byte[]? Rphoto { get; set; }

        public string? Rstatus { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();

        public int TownId { get; set; }
        public Town? Town { get; set; }

        [NotMapped]
        public List<string> Groups { get; set; } = new List<string>();

    }
}
