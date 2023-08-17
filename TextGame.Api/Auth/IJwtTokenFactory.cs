using TextGame.Data.Contracts;

namespace TextGame.Api.Auth;

public interface IJwtTokenFactory
{
    Task<string> Create(IUser user, IReadOnlyCollection<IGameAccount> gameAccounts);
}