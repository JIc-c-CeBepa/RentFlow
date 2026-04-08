using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Review
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public int UserId { get; set; }

    public int BookingId { get; set; }

    public sbyte Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool? IsPublished { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Property Property { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
