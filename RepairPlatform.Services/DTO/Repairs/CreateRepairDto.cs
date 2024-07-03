using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Repairs
{
    public class CreateRepairDto
    {
        public int RepairId { get; set; }

        public string? RepName { get; set; }

        public string? RepDescription { get; set; }
    }
}
