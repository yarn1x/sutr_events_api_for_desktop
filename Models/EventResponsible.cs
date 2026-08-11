using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class EventResponsible
{
    public int ResponsibleId { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
