using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Admins
{
    public class AdministratorDto
    { 
        public int AdministratorId { get; set; }

        public string AfullName { get; set; } = null!;

        public string Aemail { get; set; } = null!;

        public byte[] Apassword { get; set; } = null!;

    }

}
