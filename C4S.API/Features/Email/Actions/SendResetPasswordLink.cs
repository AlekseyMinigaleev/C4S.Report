using C4S.DB;
using C4S.Services.Services.EmailSenderService;
using C4S.Services.Services.TokenService;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace C4S.API.Features.Email.Actions
{
    public class SendResetPasswordLink
    {
        public class Query : IRequest
        {
            public string Email { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator(ReportDbContext dbContext)
            {
                RuleFor(x => x.Email)
                    .EmailAddress();

                RuleFor(x => x.Email)
                    .MustAsync(async (email, cancellationToken) =>
                    {
                        var user = await dbContext.Users
                            .SingleOrDefaultAsync(
                                x => x.Email == email && x.IsActive,
                                cancellationToken);

                        return user is not null;
                    })
                    .WithMessage("Пользователь с указанной почтой не был найден");
            }
        }

        public class Handler : IRequestHandler<Query>
        {
            private readonly ReportDbContext _dbContext;
            private readonly ITokenService _tokenService;
            private readonly IEmailSenderService _emailSernderService;
            private readonly string _frontBaseUrl;

            public Handler(
                ReportDbContext dbContext,
                ITokenService tokenService,
                IEmailSenderService emailService,
                IConfiguration configuration)
            {
                _dbContext = dbContext;
                _tokenService = tokenService;
                _emailSernderService = emailService;

                var frontBaseUrl = configuration["FrontBaseUrl"]!;
                if (string.IsNullOrWhiteSpace(frontBaseUrl))
                    throw new ArgumentNullException(
                        $"{nameof(frontBaseUrl)}.В конфигурации приложения не указан FrontBaseUrl");
                _frontBaseUrl = frontBaseUrl;
            }

            public async Task Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users
                    .SingleAsync(
                        x => x.Email == request.Email && x.IsActive,
                        cancellationToken);

                var token = _tokenService.GenerateToken(user.Id);

                var link = $"{_frontBaseUrl}#/reset-password?token={token}";

                await _emailSernderService.SendResetPasswordLink(
                    emailTo: user.Email,
                    subject: "Восстановление пароля",
                    resetPasswordLink: link,
                    cancellationToken);
            }
        }
    }
}