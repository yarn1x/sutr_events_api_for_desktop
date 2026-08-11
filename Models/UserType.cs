using System;
using System.Collections.Generic;

namespace college_events_admin_API.Models;

public partial class UserType
{
    public int UserTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<UserUsertype> UserUsertypes { get; set; } = new List<UserUsertype>();
}
