using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class Criterion
{
    public int CriterionId { get; set; }

    public string Name { get; set; } = null!;

    public int Score { get; set; }

    public virtual ICollection<EventCriterion> EventCriteria { get; set; } = new List<EventCriterion>();
}
