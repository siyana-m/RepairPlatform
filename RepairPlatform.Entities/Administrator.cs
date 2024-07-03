using System;
using System.Collections.Generic;

namespace RepairPlatform.Entities;

public partial class Administrator
{
    public int AdministratorId { get; set; }

    public string AfullName { get; set; } = null!;

    public string Aemail { get; set; } = null!;

    public byte[] Apassword { get; set; } = null!;

    public DateTime LastModified20118046 { get; set; }

    // Foreign Key
    public string? UserId { get; set; }

    // Navigation property
    public virtual AspNetUsers User { get; set; } = null!;
}
