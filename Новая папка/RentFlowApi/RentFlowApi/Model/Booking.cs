using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Booking
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public int UserId { get; set; }

    public int StatusId { get; set; }

    public DateOnly ArrivalDate { get; set; }

    public DateOnly DepartureDate { get; set; }

    public int GuestsCount { get; set; }

    public bool NeedsContactlessCheckin { get; set; }

    public decimal PricePerStay { get; set; }

    public decimal PrepaymentPercent { get; set; }

    public decimal PrepaymentAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? CanceledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? CanceledByUserId { get; set; }

    public string? CancellationReason { get; set; }

    public bool IsThankYouSent { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CalendarEntry> CalendarEntries { get; set; } = new List<CalendarEntry>();

    public virtual User? CanceledByUser { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Property Property { get; set; } = null!;

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    public virtual Review? Review { get; set; }

    public virtual BookingStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
