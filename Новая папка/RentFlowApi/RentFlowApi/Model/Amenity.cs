using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class Amenity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Icon { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    
}
