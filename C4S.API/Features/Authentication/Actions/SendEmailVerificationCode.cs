using C4S.DB;
using C4S.Shared.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using System.Security.Principal;

namespace C4S.API.Features.Authentication.Actions
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
            private readonly ReportDbContext _dbContext;

            public Handler(
                IPrincipal principal,
                ReportDbContext reportDbContext)
            {
                _principal = principal;
                _dbContext = reportDbContext;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();
                var user = await _dbContext.Users
                    .Include(x=>x.AuthenticationModel)
                    .SingleAsync(x => x.Id == userId, cancellationToken);

                user.AuthenticationModel?
                    .GenerateAndSetEmailVerificationCode();

                _dbContext.SaveChanges();

                /*TODO: сервис по отправке сообщений*/
            }
        }
    }
}