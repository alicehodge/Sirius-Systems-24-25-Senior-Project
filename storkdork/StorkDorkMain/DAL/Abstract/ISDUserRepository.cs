using System.Security.Claims;
using StorkDorkMain.Models;
using StorkDorkMain.Models.DTO;

namespace StorkDorkMain.DAL.Abstract;

public interface ISDUserRepository
{
    public Task<List<SdUser>> GetSdUsers();

    public Task<SdUser> GetSDUserByIdentity(ClaimsPrincipal user);
}