using TextGame.Data.Contracts;

namespace TextGame.Api.Controllers.Users;

public interface IJwtTokenFactory
{
    string Create(IUser user);
}