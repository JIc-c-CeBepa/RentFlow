using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Rule
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}
