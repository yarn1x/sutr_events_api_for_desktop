using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime StartDatetime { get; set; }

    public int? Duration { get; set; }

    public DateTime? EndDatetime { get; set; }

    public string FullDescription { get; set; } = null!;

    public string ShortDescription { get; set; } = null!;

    public int OrganizerId { get; set; }

    public string? OrganizerPosition { get; set; }

    public string? OrganizerOrganization { get; set; }

    public int CategoryId { get; set; }

    public int StatusId { get; set; }

    public int MaxListenersCount { get; set; }

    public int MaxParticipantsCount { get; set; }

    public string? AdditionalInfo { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<EventCriterion> EventCriteria { get; set; } = new List<EventCriterion>();

    public virtual ICollection<EventGroup> EventGroups { get; set; } = new List<EventGroup>();

    public virtual ICollection<EventLocation> EventLocations { get; set; } = new List<EventLocation>();

    public virtual ICollection<EventPhoto> EventPhotos { get; set; } = new List<EventPhoto>();

    public virtual ICollection<EventResponsible> EventResponsibles { get; set; } = new List<EventResponsible>();

    public virtual AuthorizedUser Organizer { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
