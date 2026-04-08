using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class ClientBehaviorEvent
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? PropertyId { get; set; }

    public int? SessionId { get; set; }

    public string EventType { get; set; } = null!;

    public DateTime EventTime { get; set; }

    public virtual Property? Property { get; set; }

    public virtual ClientSession? Session { get; set; }

    public virtual User User { get; set; } = null!;
}
