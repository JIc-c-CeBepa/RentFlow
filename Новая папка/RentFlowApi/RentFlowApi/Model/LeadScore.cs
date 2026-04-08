using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class LeadScore
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal ScorePercent { get; set; }

    public DateTime CalculatedAt { get; set; }

    public decimal SelectedDatesWeight { get; set; }

    public decimal ContactsWeight { get; set; }

    public decimal SessionDurationWeight { get; set; }

    public decimal ReturnVisitsWeight { get; set; }

    public virtual User User { get; set; } = null!;
}
