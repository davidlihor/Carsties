using System.Collections.Generic;
using System.Security.Claims;

namespace AuctionService.UnitTests.Utils;

public class Helpers
{
    public static ClaimsPrincipal GetClaimsPrincipal()
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, "user") };
        var claimsIdentity = new ClaimsIdentity(claims, "test");
        return new ClaimsPrincipal(claimsIdentity);
    }
}
