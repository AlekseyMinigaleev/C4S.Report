using C4S.DB;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using C4S.Shared.Extensions;

namespace C4S.API.Features.Authentication.Actions
{
    public class DeleteRefreshToken
    {
        public class Command : IRequest 
        { }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IPrincipal _principal;

            public Handler(
                ReportDbContext dbContext,
                IPrincipal principal)
            {
                _dbContext = dbContext;
                _principal = principal;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();

                var user = await _dbContext.Users
                    .SingleAsync(x => x.Id == userId, cancellationToken);

                user.ClearRefreshToken();

                await _dbContext.SaveChangesAsync(cancellationToken);    
            }
        }
    }
}
