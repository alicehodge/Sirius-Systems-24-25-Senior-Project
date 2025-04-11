using System.Security.Permissions;

namespace StorkDorkMain.Models;

public class MilestoneViewModel
{
    public Milestone Milestone { get; set; }
    public MostSpottedBirdDTO? MostSpottedBird { get; set; }
    public string FirstName { get; set; }
    public int SightingsTier { get; set; }
    public int PhotosTier { get; set; }
    public string GoldUrl { get; set; } = "/images/goldmedal.png";
    public string SilverUrl { get; set; } = "/images/silvermedal.png";
    public string BronzeUrl { get; set; } = "/images/bronzemedal.png";
}