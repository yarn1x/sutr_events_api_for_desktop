using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class EventGroup
{
    public int EventGroupId { get; set; }

    public int EventId { get; set; }

    public int GroupId { get; set; }

    public int ExpectedListenersCount { get; set; }

    public int ExpectedParticipantsCount { get; set; }

    public int ExpectedSuperParticipantsCount { get; set; }

    public virtual ICollection<ActualAttendance> ActualAttendances { get; set; } = new List<ActualAttendance>();

    public virtual Event Event { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
