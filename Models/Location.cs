using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string Place { get; set; } = null!;

    public bool InCollege { get; set; }

    public virtual ICollection<EventLocation> EventLocations { get; set; } = new List<EventLocation>();
}
