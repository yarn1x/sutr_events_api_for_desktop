using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class EventCriterion
{
    public int EventCriterionId { get; set; }

    public int EventId { get; set; }

    public int CriterionId { get; set; }

    public virtual Criterion Criterion { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
