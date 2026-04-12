using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentFlowApi.Model;

public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int? OwnerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MiddleName { get; set; }

    [NotMapped]
    public string? FullName { get; set; }

    public string? Login { get; set; }

    public string PasswordHash { get; set; }

    public string? Phone { get; set; }

    public string? Telegram { get; set; }

    public bool? IsActive { get; set; }

    public byte[]? Photo {  get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> BookingCanceledByUsers { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingUsers { get; set; } = new List<Booking>();

    public virtual ICollection<ClientBehaviorEvent> ClientBehaviorEvents { get; set; } = new List<ClientBehaviorEvent>();

    public virtual ClientProfile? ClientProfile { get; set; }

    public virtual ICollection<ClientSession> ClientSessions { get; set; } = new List<ClientSession>();

    public virtual ICollection<LeadScore> LeadScores { get; set; } = new List<LeadScore>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Owner? Owner { get; set; }

    public virtual ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role Role { get; set; } = null!;
}
