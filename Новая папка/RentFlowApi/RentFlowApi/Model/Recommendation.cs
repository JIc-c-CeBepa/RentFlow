using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Recommendation
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? PropertyId { get; set; }

    public int OwnerId { get; set; }

    public string Title { get; set; } = null!;

    public string RecommendationText { get; set; } = null!;

    public string ReasonText { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Owner Owner { get; set; } = null!;

    public virtual Property? Property { get; set; }

    public virtual User User { get; set; } = null!;
}
