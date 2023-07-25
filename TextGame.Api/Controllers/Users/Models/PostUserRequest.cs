using System.ComponentModel.DataAnnotations;

namespace TextGame.Api.Controllers.Users.Models;

public class PostUserRequest
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}