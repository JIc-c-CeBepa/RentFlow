using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class ClientSession
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? SourceId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public int? DurationSeconds { get; set; }

    public virtual ICollection<ClientBehaviorEvent> ClientBehaviorEvents { get; set; } = new List<ClientBehaviorEvent>();

    public virtual LeadSource? Source { get; set; }

    public virtual User User { get; set; } = null!;
}
