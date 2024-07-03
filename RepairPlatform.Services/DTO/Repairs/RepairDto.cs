using RepairPlatform.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Repairs
{
    public class RepairDto
    {
        public int RepairId { get; set; }

        public string? RepName { get; set; }

        public string? RepDescription { get; set; }

        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

        public virtual ICollection<Repairguy> Repairguys { get; set; } = new List<Repairguy>();
    }
}
