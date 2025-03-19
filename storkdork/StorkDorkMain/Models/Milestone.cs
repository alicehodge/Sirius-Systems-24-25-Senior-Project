using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StorkDorkMain.Models;

public class Milestone
{
    [Key]
    public int Id { get; set; }

    public int SDUserId { get; set; }

    public int SightingsMade { get; set; }

    public int PhotosContributed { get; set; }
}