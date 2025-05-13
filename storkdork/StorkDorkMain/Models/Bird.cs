using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorkDorkMain.Models;

public partial class Bird
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength (100)]
    public string ScientificName { get; set; } = "Scientific Name Unknown";

    [Required]
    [StringLength (100)]
    public string CommonName { get; set; } = "Common Name Unknown";

    [Required]
    [StringLength (10)]
    public string SpeciesCode { get; set; } = "Species Code Unknown";

    [Required]
    [StringLength (10)]
    public string Category { get; set; } = "Category Unknown";

    [StringLength (25)]
    public string? Order { get; set; }

    [StringLength (50)]
    public string? FamilyCommonName { get; set; }

    [StringLength (50)]
    public string? FamilyScientificName { get; set; }

    [StringLength (10)]
    public string? ReportAs { get; set; }

    [StringLength (1000)]
    public string? Range { get; set; }

    public virtual ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();

    public virtual ICollection<Sighting> Sightings { get; set; } = new List<Sighting>();
    
    public virtual ICollection<BirdPhoto> Photos { get; set; } = new List<BirdPhoto>();

    // Navigation property for related birds
    [NotMapped]
    public List<Bird> RelatedBirds { get; set; } = new List<Bird>();

}