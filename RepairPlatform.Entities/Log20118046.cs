using System;
using System.Collections.Generic;

namespace RepairPlatform.Entities;

public partial class Log20118046
{
    public int LogId { get; set; }

    public string TableName { get; set; } = null!;

    public string OperationType { get; set; } = null!;

    public DateTime OperationDateTime { get; set; }

    public int? AdministratorId { get; set; }
}
