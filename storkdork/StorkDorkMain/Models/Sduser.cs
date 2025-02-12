using System;
using System.Collections.Generic;

namespace StorkDorkMain.Models;

public partial class SdUser
{
    public int Id { get; set; }

    public string? AspNetIdentityId { get; set; }

    public virtual ICollection<Checklist> Checklists { get; set; } = new List<Checklist>();

    public virtual ICollection<Sighting> Sightings { get; set; } = new List<Sighting>();
}
