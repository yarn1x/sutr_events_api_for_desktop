using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class EventLocation
{
    public int EventLocationId { get; set; }

    public int EventId { get; set; }

    public int LocationId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;
}
