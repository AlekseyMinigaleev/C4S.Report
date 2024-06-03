using C4S.DB;
using C4S.Services.Services.EmailSenderService;
using C4S.Shared.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace C4S.API.Features.Email.Actions
{
    public class SendEmailVerificationCode
    {
        public class Command : IRequest
        {
            public string Email { get; set; }
        }

        public class EmailValidator : AbstractValidator<Command>
        {
            public EmailValidator(
                IPrincipal principal,
                ReportDbContext dbContext)
            {
                RuleFor(x => x)
                    .MustAsync(async (x, cancellationToken) =>
                    {
                        /*TODO: этот код очень часто повторяется нужно вынести в отдельный метод*/
                        var userId = principal.GetUserId();
                        var user = await dbContext.Users
                            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);

                        return user is not null;
                    });
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IPrincipal _principal;
            private readonly IEmailSenderService _emailSenderService;
            private readonly ReportDbContext _dbContext;

            public Handler(
                IPrincipal principal,
                IEmailSenderService emailSenderService,
                ReportDbContext reportDbContext)
            {
                _emailSenderService = emailSenderService;
                _principal = principal;
                _dbContext = reportDbContext;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();
                var userAuthenticationModel = await _dbContext.UserAuthenticationModels
                    .Include(x => x.User)
                    .SingleAsync(x => x.UserId == userId, cancellationToken);

                userAuthenticationModel
                    .GenerateAndSetEmailVerificationCode();

                _dbContext.SaveChanges();

                await _emailSenderService.SendVerificationCode(
                    userAuthenticationModel.User.Email,
                    "Код подтверждения почты",
                     Convert.ToInt32(userAuthenticationModel.EmailVerificationCode!.Token),
                    cancellationToken);
            }
        }
    }
}