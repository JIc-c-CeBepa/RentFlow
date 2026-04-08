using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class ClientProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? LeadSourceId { get; set; }

    public DateTime? FirstVisitAt { get; set; }

    public DateTime? LastVisitAt { get; set; }

    public int VisitsCount { get; set; }

    public bool HasSelectedDates { get; set; }

    public bool HasLeftContacts { get; set; }

    public decimal LeadScorePercent { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual LeadSource? LeadSource { get; set; }

    public virtual User User { get; set; } = null!;
}
