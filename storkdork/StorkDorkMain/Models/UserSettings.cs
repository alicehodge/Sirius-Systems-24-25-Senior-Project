using System.Security.Cryptography.X509Certificates;

namespace StorkDorkMain.Models;

public class UserSettings
{
    public int Id {get;set;}
    public int SdUserId {get;set;}
    public bool AnonymousSightings {get;set;}

    // public virtual SdUser? SdUser {get;set;}
}