using System.ComponentModel.DataAnnotations;

namespace TextGame.Api.Controllers.Users.Models;

public class PostUserRequest
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}

public record PostUserSearchRequest(
    [Required] string Text,
    [Range(1, 100)] int Limit = 100);