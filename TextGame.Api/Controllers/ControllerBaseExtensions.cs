﻿using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TextGame.Api.Auth;
using TextGame.Data.Contracts;

namespace TextGame.Api.Controllers;

public static class ControllerBaseExtensions
{
    public static AuthTicket GetTicket(this ControllerBase self)
    {
        var key = self.HttpContext.User.Claims.GetClaimOrThrow(JwtRegisteredClaimNames.Sub);

        return new AuthTicket(DateTimeOffset.UtcNow, key);
    }

    public static string GetLocale(this ControllerBase self)
    {
        return self.HttpContext.Items["locale"]?.ToString() ?? throw new InvalidOperationException("Locale not found");
    }
}

