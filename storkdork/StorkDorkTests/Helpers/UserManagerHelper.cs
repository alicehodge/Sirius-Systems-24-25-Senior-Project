using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace StorkDorkTests.Helpers;
public static class UserManagerHelper
{
    public static Mock<UserManager<IdentityUser>> GetMockUserManager()
    {
        var mockOptions = new Mock<IOptions<IdentityOptions>>();
        var mockUserStore = new Mock<IUserStore<IdentityUser>>();
        var mockPasswordHasher = new Mock<IPasswordHasher<IdentityUser>>();
        var mockUserValidator = new Mock<IUserValidator<IdentityUser>>();
        var mockPasswordValidator = new Mock<IPasswordValidator<IdentityUser>>();
        var mockLookupNormalizer = new Mock<ILookupNormalizer>();
        var mockIdentityErrorDescriber = new Mock<IdentityErrorDescriber>();
        var mockLogger = new Mock<ILogger<UserManager<IdentityUser>>>();
        var mockClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

        var mockUserManager = new Mock<UserManager<IdentityUser>>(
            mockUserStore.Object,
            mockOptions.Object,
            mockPasswordHasher.Object,
            new[] { mockUserValidator.Object },
            new[] { mockPasswordValidator.Object },
            mockLookupNormalizer.Object,
            mockIdentityErrorDescriber.Object,
            mockLogger.Object,
            mockClaimsPrincipalFactory.Object
        );

        return mockUserManager;
    }
}