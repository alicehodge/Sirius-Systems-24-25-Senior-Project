using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorkDorkMain.Models;

[Table("SDUser")]
public partial class SdUser
{
    public int Id { get; set; }

    public string? AspNetIdentityId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    // public virtual ICollection<Checklist> Checklists { get; set; } = new List<Checklist>();

    // public virtual ICollection<Sighting> Sightings { get; set; } = new List<Sighting>();
}
