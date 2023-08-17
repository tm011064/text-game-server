using MediatR;
using TextGame.Data.Contracts;

namespace TextGame.Core.Users.Events;

public record CreateUserRequest(string Email, string Password, IReadOnlySet<string> Roles, AuthTicket Ticket) : IRequest<IUser>;
