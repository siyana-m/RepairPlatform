using RepairPlatform.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Towns
{
    public class TownDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime LastModified20118046 { get; set; }

        public virtual ICollection<Repairguy> Repairguys { get; set; } = new List<Repairguy>();
    }
}
