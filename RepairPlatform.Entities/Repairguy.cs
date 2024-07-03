using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairPlatform.Entities;

public partial class Repairguy
{
    public int RepairguyId { get; set; }

    public string RfirstName { get; set; } = null!;

    public string RlastName { get; set; } = null!;

    public string Rtelephone { get; set; } = null!;

    public string Remail { get; set; } = null!;

    public string Rpassword { get; set; } = null!;

    public string? Rdescription { get; set; }

    public byte[]? Rphoto { get; set; }

    public string? Rstatus { get; set; }

    public DateTime LastModified20118046 { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();


    
    [NotMapped]
    public List<string> Groups { get; set; } = new List<string>();

    
    public string UserId { get; set; } = string.Empty!;

    public virtual AspNetUsers User { get; set; } = null!;


    public int TownId { get; set; }

    public virtual Town Town { get; set; } = null!; 
}
