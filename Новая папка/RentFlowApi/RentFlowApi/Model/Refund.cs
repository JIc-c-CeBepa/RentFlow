using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Refund
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Payment Payment { get; set; } = null!;
}
