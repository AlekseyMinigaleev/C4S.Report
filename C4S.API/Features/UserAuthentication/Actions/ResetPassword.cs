using C4S.DB;
using C4S.Services.Services.TokenService;
using C4S.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace C4S.API.Features.UserAuthentication.Actions
{
    public class ResetPassword
    {
        public class Command : IRequest
        {
            public string Password { get; set; }

            public string ResetPasswordToken { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator(
                ITokenService tokenService,
                IConfiguration configuration,
                ReportDbContext dbContext)
            {
                /*TODO: в конфигурацию можно вынести + DRY*/
                var emailTokensExpirationModel = configuration
                    .GetSection("EmailTokensExpiration")
                    .Get<EmailTokensExpiration>();

                DateTime _creationDate;

                /*TODO: исправить, скопипастил с UserCredentionalsValidator*/
                RuleFor(x => x.Password)
                .MinimumLength(8)
                    .WithMessage("Минимальная длина пароля - 8 символов")
                .Matches("[A-Z]")
                    .WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
                .Matches("[a-z]")
                    .WithMessage("Пароль должен содержать хотя бы одну строчную букву")
                .Matches("[0-9]")
                    .WithMessage("Пароль должен содержать хотя бы одну цифру");

                RuleFor(x => x.ResetPasswordToken)
                    .NotNull()
                    .NotEmpty()
                    .MustAsync(async (x, cancellationToken) =>
                    {
                        var (userId, creationDate) = await tokenService
                            .DecryptTokenAsync(x, cancellationToken);

                        _creationDate = creationDate;

                        var user = await dbContext.Users
                            .SingleOrDefaultAsync(
                                x => x.Id == userId,
                                cancellationToken);

                        if (user is null)
                            return false;

                        var tokenExpirationDate = creationDate
                            .AddMinutes(emailTokensExpirationModel!.ResetPasswordTokenInMinutes);
                        var result = tokenExpirationDate > DateTime.UtcNow;

                        return result;
                    })
                    .WithMessage("Вышло время действия токена.");
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ReportDbContext _dbContext;
            private readonly ITokenService _tokenService;

            public Handler(
                ReportDbContext dbContext,
                ITokenService tokenService)
            {
                _dbContext = dbContext;
                _tokenService = tokenService;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var (userId, creationDate) = await _tokenService
                            .DecryptTokenAsync(request.ResetPasswordToken, cancellationToken);

                var userAuthentication = await _dbContext.UserAuthenticationModels
                    .SingleAsync(
                        x => x.UserId == userId,
                        cancellationToken);

                userAuthentication
                    .SetPassword(request.Password);

                userAuthentication
                    .SetRefreshToken(null);

                _dbContext.SaveChanges();
            }
        }
    }
}