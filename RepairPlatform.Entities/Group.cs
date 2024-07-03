using System;
using System.Collections.Generic;

namespace RepairPlatform.Entities;

public partial class Group
{
    public int GroupId { get; set; }

    public string CatName { get; set; } = null!;

    public string? CatDescription { get; set; }

    public DateTime LastModified20118046 { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}
