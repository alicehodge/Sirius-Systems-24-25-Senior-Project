using System;
using System.Collections.Generic;

namespace StorkDorkMain.Models;

public partial class Checklist
{
    public int Id { get; set; }

    public string? ChecklistName { get; set; }

    public int? SdUserId { get; set; }

    public virtual ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();

    public virtual SdUser? SdUser { get; set; }
}
