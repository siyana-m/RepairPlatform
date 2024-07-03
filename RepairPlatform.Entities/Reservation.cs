using System;
using System.Collections.Generic;

namespace RepairPlatform.Entities;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int ClientId { get; set; }

    public int RepairguyId { get; set; }

    public int GroupId { get; set; }

    public string ResName { get; set; } = null!;

    public DateTime ResDateTime { get; set; }

    public string ResLocation { get; set; } = null!;

    public string? ResComment { get; set; }

    public string? ResStatus { get; set; }

    public DateTime LastModified20118046 { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual Repairguy Repairguy { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
