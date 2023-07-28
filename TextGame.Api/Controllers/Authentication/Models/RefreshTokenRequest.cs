using System.ComponentModel.DataAnnotations;

namespace TextGame.Api.Controllers.Authentication.Models;

public record RefreshTokenRequest(
    [Required] string? Token,
    [Required] string? RefreshToken);
