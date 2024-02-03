﻿using C4S.DB;
using C4S.Services.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using С4S.API.Features.Authentication.Models;
using С4S.API.Features.Authentication.ViewModels;

namespace С4S.API.Features.Authentication.Actions
{
    public class Login
    {
        public class Query : IRequest<ResponseViewModel>
        {
            /// <inheritdoc cref="UserCredentionals"/>
            public UserCredentionals UserCreditionals { get; set; }
        }

        public class ResponseViewModel
        {
            public AuthorizationTokens AuthorizationTokens { get; set; }

            public string DeveloperName { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(ReportDbContext dbContext)
            {
                RuleFor(x => x.UserCreditionals)
                .Cascade(CascadeMode.Stop)
                .SetValidator(new UserCredentionalsValidator())
                .MustAsync(async (query, cancellationToken) =>
                {
                    var user = await dbContext.Users
                        .SingleOrDefaultAsync(
                            x => x.Login.Equals(query.Login) && x.Password.Equals(query.Password),
                            cancellationToken);

                    return user is not null;
                })
                .WithErrorCode(HttpStatusCode.NotFound.ToString())
                .WithMessage("Введены неверные логин или пароль");
            }
        }

        private class Handler : IRequestHandler<Query, ResponseViewModel>
        {
            private readonly IJwtService _jwtService;
            private readonly ReportDbContext _dbContext;

            public Handler(
                ReportDbContext dbContext,
                IJwtService jwtService)
            {
                _jwtService = jwtService;
                _dbContext = dbContext;
            }

            public async Task<ResponseViewModel> Handle(
                Query query,
                CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users
                    .SingleAsync(
                        x => x.Login.Equals(query.UserCreditionals.Login),
                        cancellationToken);

                var developerName = user.GetDeveloperName();

                var authorizationTokens = new AuthorizationTokens
                {
                    AccessToken = _jwtService.CreateJwtToken(user, _jwtService.AccessTokenExpiry),
                    RefreshToken = _jwtService.CreateJwtToken(user, _jwtService.RefreshTokenExpiry),
                };

                user.SetRefreshToken(authorizationTokens.RefreshToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var response = new ResponseViewModel
                {
                    DeveloperName = developerName,
                    AuthorizationTokens = authorizationTokens,
                };

                return response;
            }
        }
    }
}