using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Property
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public string Title { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public decimal CurrentPrice { get; set; }

    public int MaxGuests { get; set; }

    public decimal PrepaymentPercent { get; set; }

    public string BookingMode { get; set; } = null!;

    public bool IsContactlessCheckinAvailable { get; set; }

    public TimeOnly? CheckInTime { get; set; }

    public TimeOnly? CheckOutTime { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CalendarEntry> CalendarEntries { get; set; } = new List<CalendarEntry>();

    public virtual ICollection<ClientBehaviorEvent> ClientBehaviorEvents { get; set; } = new List<ClientBehaviorEvent>();

    public virtual Owner Owner { get; set; } = null!;

    public virtual ICollection<PropertyPhoto> PropertyPhotos { get; set; } = new List<PropertyPhoto>();

    public virtual ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<SeasonalPriceRule> SeasonalPriceRules { get; set; } = new List<SeasonalPriceRule>();

    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();

    public virtual ICollection<Rule> Rules { get; set; } = new List<Rule>();
}
