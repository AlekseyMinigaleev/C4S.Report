using AngleSharp;
using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Services.JWTService;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using C4S.API.Features.Authentication.Models;
using C4S.API.Features.Authentication.ViewModels;

namespace C4S.API.Features.Authentication.Actions
{
    public class Login
    {
        public class Query : IRequest<ResponseViewModel>
        {
            /// <inheritdoc cref="UserCredentials"/>
            public UserCredentials UserCreditionals { get; set; }
        }

        public class ResponseViewModel
        {
            public AuthorizationTokens AuthorizationTokens { get; set; }

            public DeveloperInfoViewModel DeveloperInfo { get; set; }
        }

        public class DeveloperInfoViewModel
        {
            public string DeveloperName { get; set; }
            public string DeveloperPageUrl { get; set; }
            public bool IsRsyaAuthorizationTokenSet { get; set; }
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
                        .Include(x=>x.AuthenticationModel)
                        .SingleOrDefaultAsync(
                            x => x.Email.Equals(query.Login)
                                && x.AuthenticationModel.ValidatePassword(query.Password),
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
            private readonly IBrowsingContext _browsingContext;
            private readonly ReportDbContext _dbContext;

            public Handler(
                ReportDbContext dbContext,
                IJwtService jwtService,
                IBrowsingContext browsingContext)
            {
                _jwtService = jwtService;
                _dbContext = dbContext;
                _browsingContext = browsingContext;
            }

            public async Task<ResponseViewModel> Handle(
                Query query,
                CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users
                    .Include(x=>x.AuthenticationModel)
                    .SingleAsync(
                        x => x.Email.Equals(query.UserCreditionals.Login),
                        cancellationToken);

                var authorizationTokens =
                    await CreateAuthorizationTokensAndUpdateDBAsync(user, cancellationToken);

                var developerInfo = await CreateDeveloperInfoViewModel(user, cancellationToken);

                var response = new ResponseViewModel
                {
                    AuthorizationTokens = authorizationTokens,
                    DeveloperInfo = developerInfo,
                };

                return response;
            }

            private async Task<AuthorizationTokens> CreateAuthorizationTokensAndUpdateDBAsync(
                UserModel user,
                CancellationToken cancellationToken)
            {
                var authorizationTokens = new AuthorizationTokens
                {
                    AccessToken = _jwtService.CreateJwtToken(user, _jwtService.AccessTokenExpiry),
                    RefreshToken = _jwtService.CreateJwtToken(user, _jwtService.RefreshTokenExpiry),
                };

                user.AuthenticationModel
                    .SetRefreshToken(authorizationTokens.RefreshToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return authorizationTokens;
            }

            private async Task<DeveloperInfoViewModel> CreateDeveloperInfoViewModel(
                UserModel user,
                CancellationToken cancellationToken)
            {
                var document = await _browsingContext
                    .OpenAsync("https://yandex.ru/games/developer/42543", cancellationToken);

                /*TODO: сделать общую ошибку для случая когда с парсинга приходит null*/
                var developerCard = document.QuerySelector(".developer-card__name") ?? throw new ArgumentNullException();
                var developerName = developerCard.TextContent;

                var developerInfo = new DeveloperInfoViewModel
                {
                    DeveloperPageUrl = user.DeveloperPageUrl,
                    DeveloperName = developerName,
                    IsRsyaAuthorizationTokenSet = user.RsyaAuthorizationToken is not null,
                };

                return developerInfo;
            }
        }
    }
}