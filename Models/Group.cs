using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class Group
{
    public int GroupId { get; set; }

    public string Name { get; set; } = null!;

    public int AuthorizedUserId { get; set; }

    public int StudentsCount { get; set; }

    public DateOnly creationDate { get; set; }

    public virtual AuthorizedUser? AuthorizedUser { get; set; }

    public virtual ICollection<EventGroup> EventGroups { get; set; } = new List<EventGroup>();
}
