using MediatR;
using TextGame.Data.Contracts;

namespace TextGame.Core.Users.Events;

public record CreateUserRequest(string Email, string Password, AuthTicket Ticket) : IRequest<IUser>;
