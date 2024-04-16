﻿using C4S.DB;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using С4S.API.Features.User.Requests;
using C4S.Shared.Extensions;

namespace С4S.API.Features.User.Action
{
    public class SetRsyaAuthorizationToken
    {
        public class Command : IRequest
        {
            /// <inheritdoc cref="SetRsyaAuthorizationToken"/>
            public RsyaAuthorizationToken RsyaAythorizationToken { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(IHttpClientFactory httpClientFactory)
            {
                RuleFor(x => x.RsyaAythorizationToken)
                    .SetValidator(new RsyaAuthorizationTokenValidator(httpClientFactory));
            }
        }

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
                var userid = _principal.GetUserId();

                var user = await _dbContext.Users
                    .SingleAsync(x => x.Id == userid, cancellationToken);

                user.SetAuthorizationToken(request.RsyaAythorizationToken.Token);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}