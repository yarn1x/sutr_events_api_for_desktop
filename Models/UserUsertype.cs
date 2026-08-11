using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class UserUsertype
{
    public int UserUsertypeId { get; set; }

    public int UserTypeId { get; set; }

    public int AuthorizedUserId { get; set; }

    public virtual AuthorizedUser AuthorizedUser { get; set; } = null!;

    public virtual UserType UserType { get; set; } = null!;
}
