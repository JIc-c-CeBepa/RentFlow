using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? BookingId { get; set; }

    public string Type { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User User { get; set; } = null!;
}
