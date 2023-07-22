using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using TextGame.Core.Cryptography;
using TextGame.Data;
using TextGame.Data.Contracts;
using TextGame.Data.Queries.Users;

namespace TextGame.Api
{
    //https://jasonwatmore.com/net-7-csharp-jwt-authentication-tutorial-without-aspnet-core-identity
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                return;
            }

            if (context.HttpContext.Items["User"] is not IUser)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtUtils.ValidateJwtToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                // TODO (Roman): do this!!  
                //context.Items["User"] = userService.GetById(userId.Value);
            }

            await _next(context);
        }
    }

    public interface IJwtUtils
    {
        public string GenerateJwtToken(IUser user);
        public int? ValidateJwtToken(string? token);
    }
    public interface IUserService
    {
        Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
    }

    public class UserService : IUserService
    {
        private readonly IQueryService queryService;

        private readonly Rfc2898PasswordValidator validator = new();

        private readonly IJwtUtils jwtUtils;

        public UserService(IQueryService queryService, IJwtUtils jwtUtils)
        {
            this.queryService = queryService;
            this.jwtUtils = jwtUtils;
        }

        public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest request)
        {
            var user = await queryService.Run(new GetUserByEmail(request.Email!));

            // return null if user not found
            if (user == null)
            {
                return null; // throw exception?
            }

            var password = await queryService.Run(new GetUserPassword(user.Id));

            if (!validator.IsValid(request.Password!, password))
            {
                return null; // throw exception?
            }

            var token = jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }
    }

    public class JwtUtils : IJwtUtils
    {
        private readonly string secretKey;

        public JwtUtils(IConfiguration configuration)
        {
            secretKey = configuration.GetValue<string>("TokenAuthentication:SecretKey")
                ?? throw new Exception("JWT secret not configured");
        }

        public string GenerateJwtToken(IUser user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateJwtToken(string? token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch (Exception exception)
            {
                // return null if validation fails
                return null;
            }
        }
    }

    public class AuthenticateResponse
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public AuthenticateResponse(IUser user, string token)
        {
            Id = user.Id;
            Token = token;
        }
    }

    public class AuthenticateRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
