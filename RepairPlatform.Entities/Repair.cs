using System;
using System.Collections.Generic;

namespace RepairPlatform.Entities;

public partial class Repair
{
    public int RepairId { get; set; }

    public string RepName { get; set; } = null!;

    public string? RepDescription { get; set; }

    public DateTime LastModified20118046 { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Repairguy> Repairguys { get; set; } = new List<Repairguy>();
}
