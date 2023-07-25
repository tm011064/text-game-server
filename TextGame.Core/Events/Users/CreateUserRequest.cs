using MediatR;
using TextGame.Data.Contracts;

namespace TextGame.Core.Events.Users;

public record CreateUserRequest(string Email, string Password, AuthTicket Ticket) : IRequest<IUser>;
