using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Owner
{
    public int Id { get; set; }

    public string? CompanyName { get; set; }

    public string? Phone { get; set; }

    public string? Telegram { get; set; }

    public string? Email { get; set; }

    public string? PublicSlug { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
