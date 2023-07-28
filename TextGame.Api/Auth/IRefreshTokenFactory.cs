using TextGame.Data.Contracts;

namespace TextGame.Api.Auth;

public interface IRefreshTokenFactory
{
    Task<string> Create(IUser user);
}