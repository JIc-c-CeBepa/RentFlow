using System;
using System.Collections.Generic;

namespace RentFlowApi.Model;

public partial class LeadSource
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ClientProfile> ClientProfiles { get; set; } = new List<ClientProfile>();

    public virtual ICollection<ClientSession> ClientSessions { get; set; } = new List<ClientSession>();
}
