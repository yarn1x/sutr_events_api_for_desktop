using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class EventPhoto
{
    public int EventPhotoId { get; set; }

    public int EventId { get; set; }

    public string PhotoUrl { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
