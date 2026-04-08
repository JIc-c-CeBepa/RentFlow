using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class CalendarEntry
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public int? BookingId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string EntryType { get; set; } = null!;

    public bool? IsBlocked { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Property Property { get; set; } = null!;
}
