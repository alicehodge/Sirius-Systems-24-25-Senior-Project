using System;
using System.Collections.Generic;

namespace StorkDorkMain.Models;

public partial class Checklist
{
    public int Id { get; set; }

    public string? ChecklistName { get; set; }

    public int? SduserId { get; set; }

    public virtual ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();

    public virtual Sduser? Sduser { get; set; }
}
