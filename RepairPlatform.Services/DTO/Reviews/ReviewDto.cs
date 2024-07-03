using RepairPlatform.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.DTO.Reviews
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }

        public int RepairguyId { get; set; }

        public int ClientId { get; set; }

        public int GroupId { get; set; }

        public int Rating { get; set; }

        public DateTime RevDateTime { get; set; }

        public string RevLocation { get; set; } = null!;

        public string? RevComment { get; set; }

        public virtual Client Client { get; set; } = null!;

        public virtual Group Group { get; set; } = null!;

        public virtual Repairguy Repairguy { get; set; } = null!;
    }
}
