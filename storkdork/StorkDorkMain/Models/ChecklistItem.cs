using System;
using System.Collections.Generic;

namespace StorkDorkMain.Models;

public partial class ChecklistItem
{
    public int Id { get; set; }

    public int? ChecklistId { get; set; }

    public int? BirdId { get; set; }

    public bool? Sighted { get; set; }

    public virtual Bird? Bird { get; set; }

    public virtual Checklist? Checklist { get; set; }
}
