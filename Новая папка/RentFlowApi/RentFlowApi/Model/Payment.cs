using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Payment
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? PaidAt { get; set; }

    public string? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
