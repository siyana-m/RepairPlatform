using RepairPlatform.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Groups
{
    public class GroupDto
    {
        public int GroupId { get; set; }

        public string CatName { get; set; } = null!;

        public string? CatDescription { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    }
}
