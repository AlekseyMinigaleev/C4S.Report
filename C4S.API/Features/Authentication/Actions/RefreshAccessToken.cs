using C4S.DB;
using C4S.Services.Services.JWTService;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace C4S.API.Features.Authentication.Actions
{
    public class RefreshAccessToken
    {
        public class Command : IRequest<string>
        {
            public string RefreshToken { get; set; }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IJwtService _jwtService;

            public Handler(
                ReportDbContext dbContext,
                IJwtService jwtService)
            {
                _dbContext = dbContext;
                _jwtService = jwtService;
            }

            public async Task<string?> Handle(Command request, CancellationToken cancellationToken)
            {
                var refreshToken = request.RefreshToken;
                var handler = new JwtSecurityTokenHandler();

                var isExpired = ValidateTokenExpiry(refreshToken, handler);
                if (isExpired)
                    return null;

                var login = GetUserLogin(refreshToken, handler);
                if (login is null)
                    return null;

                var user = await _dbContext.Users
                    .SingleOrDefaultAsync(x => x.Login.Equals(login), cancellationToken);
                if (user is null
                    || user.RefreshToken != refreshToken)
                    return null;

                var accessToken = _jwtService
                    .CreateJwtToken(user, _jwtService.AccessTokenExpiry);

                return accessToken;
            }

            private string? GetUserLogin(string refreshToken, JwtSecurityTokenHandler handler)
            {
                var validation = new TokenValidationParameters()
                {
                    ValidIssuer = _jwtService.JwtConfig.Issuer,
                    ValidAudience = _jwtService.JwtConfig.Audience,
                    IssuerSigningKey = _jwtService.SymmetricSecurityKey,
                    ValidateLifetime = false,
                };

                var principal = handler.ValidateToken(refreshToken, validation, out _);

                if (principal?.Identity?.Name is null)
                    return null;

                return principal.Identity.Name;
            }

            private static bool ValidateTokenExpiry(string refreshToken, JwtSecurityTokenHandler handler)
            {
                if (handler.ReadToken(refreshToken) is not JwtSecurityToken refreshJsonToken)
                    return false;

                var isExpired = refreshJsonToken?.ValidTo < DateTime.Now;

                return isExpired;
            }
        }
    }
}