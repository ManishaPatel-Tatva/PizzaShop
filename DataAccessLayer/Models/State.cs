using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models;

public partial class State
{
    public long Id { get; set; }

    public long? CountryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Country? Country { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
