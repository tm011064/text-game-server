using TextGame.Data.Contracts;

namespace TextGame.Api.Auth;

public record UserIdentity(string Key) : IUserIdentity;