using System.ComponentModel.DataAnnotations;

namespace TextGame.Api.Controllers.Authentication.Models;

public record AuthenticateRequest(
    [Required] string? Email,
    [Required] string? Password);
