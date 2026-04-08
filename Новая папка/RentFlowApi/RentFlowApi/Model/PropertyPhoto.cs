using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class PropertyPhoto
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public string? PublicUrl { get; set; }

    public bool IsMain { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Property Property { get; set; } = null!;
}
