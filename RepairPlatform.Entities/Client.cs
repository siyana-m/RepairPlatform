using System;
using System.Collections.Generic;

namespace RepairPlatform.Entities;

public partial class Client
{
    public int ClientId { get; set; }

    public string CfirstName { get; set; } = null!;

    public string ClastName { get; set; } = null!;

    public string Ctelephone { get; set; } = null!;

    public string Cemail { get; set; } = null!;

    public string Cpassword { get; set; } = null!;

    public byte[]? Cphoto { get; set; }

    public string? Cstatus { get; set; }

    public DateTime LastModified20118046 { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    // Foreign Key
    public string? UserId { get; set; }

    // Navigation property
    public virtual AspNetUsers User { get; set; } = null!;
}
