﻿using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace TextGame.Api.Auth;

public static class JwtClaimExtensions
{
    public static string GetClaimOrThrow(this IEnumerable<Claim> self, string claimType)
    {
        return self.FirstOrDefault(x => x.Type == claimType)?.Value
            ?? throw new SecurityTokenException($"Claim {claimType} not found");
    }

    public static string? MaybeGetClaim(this IEnumerable<Claim> self, string claimType)
    {
        return self.FirstOrDefault(x => x.Type == claimType)?.Value;
    }
}