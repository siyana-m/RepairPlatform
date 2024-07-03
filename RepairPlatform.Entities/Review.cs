using System;
using System.Collections.Generic;

namespace RepairPlatform.Entities;

public partial class Review
{
    public int ReviewId { get; set; }

    public int RepairguyId { get; set; }

    public int ClientId { get; set; }

    public int GroupId { get; set; }

    public int Rating { get; set; }

    public DateTime RevDateTime { get; set; }

    public string RevLocation { get; set; } = null!;

    public string? RevComment { get; set; }

    public DateTime LastModified20118046 { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual Repairguy Repairguy { get; set; } = null!;

    public int? ReservationId { get; set; } // Add this line

    public virtual Reservation? Reservation { get; set; } // Add this line

}
