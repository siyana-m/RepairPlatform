using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Entities
{
    public class AspNetUsers : IdentityUser
    {
        public virtual Repairguy Repairguys { get; set; } = null!;

        public virtual Client Clients { get; set; } = null!;

        //public byte[]? ProfilePicture { get; set; }
    }
}
