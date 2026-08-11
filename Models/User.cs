using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public virtual ICollection<AuthorizedUser> AuthorizedUsers { get; set; } = new List<AuthorizedUser>();

    public virtual ICollection<EventResponsible> EventResponsibles { get; set; } = new List<EventResponsible>();
}
