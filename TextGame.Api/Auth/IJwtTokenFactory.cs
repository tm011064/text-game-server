using TextGame.Data.Contracts;

namespace TextGame.Api.Auth;

public interface IJwtTokenFactory
{
    string Create(IUser user);
}