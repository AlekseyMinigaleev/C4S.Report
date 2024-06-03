using C4S.DB;
using C4S.DB.Models;
using C4S.Shared.Extensions;
using C4S.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Security.Principal;

namespace C4S.API.Features.Email.Actions
{
    public class VerifyEmailVerificationCode
    {
        public class Query : IRequest<Response>
        {
            public string VerificationCode { get; set; }
        }

        public class Response
        {
            public bool IsSuccessCode { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private string _existEmailVerificationCodeErrorMessage { get; set; } = "123";
            private UserAuthenticationModel? _userAuthenticationModel;
            private EmailTokensExpiration? _emailTokensExpirationModel;

            public QueryValidator(
                IPrincipal principal,
                IConfiguration configuration,
                ReportDbContext dbContext)
            {
                var userId = principal.GetUserId();
                _userAuthenticationModel = dbContext.UserAuthenticationModels
                    .SingleOrDefault(x => x.UserId == userId);

                _emailTokensExpirationModel = configuration
                            .GetSection("EmailTokensExpiration")
                            .Get<EmailTokensExpiration>();

                RuleFor(x => x.VerificationCode)
                    .Matches("^\\d{6}$")
                    .WithMessage("Код подтверждения должен состоять из 6 цифр.");

                RuleFor(x => x)
                    .Must(x =>
                    {
                        var result = _userAuthenticationModel?.EmailVerificationCode is not null
                            || _userAuthenticationModel?.EmailVerificationCode?.Token is not null;
                        return result;
                    })
                    .WithMessage("У пользователя отсутствуют аутентификационные данные.");

                RuleFor(x => x)
                    .Must(x =>
                    {
                        if (_emailTokensExpirationModel?.EmailVerificationCodeInMinutes is null)
                            throw new ConfigurationErrorsException(
                                "Конфигурация, времени жизни кода подтверждения почты не установлена.");

                        return true;
                    })
                    .WithMessage("Конфигурация, времени жизни кода подтверждения почты не установлена.");

                RuleFor(x => x)
                    .Must(x =>
                    {
                        var codeCreationDate = _userAuthenticationModel!.EmailVerificationCode!.CreationDate;

                        var codeExpirationDate = codeCreationDate
                            .AddMinutes(_emailTokensExpirationModel!.EmailVerificationCodeInMinutes);

                        return codeExpirationDate > DateTime.UtcNow;
                    })
                    .WithMessage("Вышло время подтверждения кода.");
            }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IPrincipal _principal;
            private readonly ReportDbContext _dbContext;

            public Handler(
                IPrincipal principal,
                ReportDbContext dbContext)
            {
                _dbContext = dbContext;
                _principal = principal;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();
                var userAuthenticationModel = await _dbContext.UserAuthenticationModels
                    .SingleAsync(x => x.UserId == userId, cancellationToken);

                var result = userAuthenticationModel!.EmailVerificationCode!.Token == request.VerificationCode;

                if (result)
                {
                    userAuthenticationModel!.EmailVerificationCode = null;
                    var user = await _dbContext.Users
                        .SingleAsync(
                            x => x.Id == userAuthenticationModel.UserId,
                            cancellationToken);
                    /*TODO: Перенести это в последний этап регистрации пользователя, т.е. после выполнения синхронизации*/
                    user.IsActive = true;

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                return new Response
                {
                    IsSuccessCode = result
                };
            }
        }
    }
}