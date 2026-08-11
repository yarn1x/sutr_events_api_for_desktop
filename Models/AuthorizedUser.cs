using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class AuthorizedUser
{
    public int AuthorizedUserId { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserPhoto> UserPhotos { get; set; } = new List<UserPhoto>();

    public virtual ICollection<UserUsertype> UserUsertypes { get; set; } = new List<UserUsertype>();
}
