using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class SeasonalPriceRule
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal? PriceMultiplier { get; set; }

    public decimal? FixedPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Property Property { get; set; } = null!;
}
