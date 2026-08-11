using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class News
{
    public int NewsId { get; set; }

    public string Headline { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
