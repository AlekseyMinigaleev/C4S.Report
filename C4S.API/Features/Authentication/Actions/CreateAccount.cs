﻿using C4S.API.Features.Authentication.ViewModels;
using C4S.API.Features.User.Requests;
using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Services.BackgroundJobService;
using C4S.Services.Services.EmailSenderService;
using C4S.Services.Services.GameSyncService;
using C4S.Shared.Logger;
using C4S.Shared.Utils;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text.Json.Serialization;

namespace C4S.API.Features.Authentication.Actions
{
    public class CreateAccount
    {
        public class Query : IRequest
        {
            /// <inheritdoc cref="UserCredentials"/>
            public UserCredentials Credentionals { get; set; }

            /// <summary>
            /// ссылка на страницу разработчика
            /// </summary>
            public string DeveloperPageUrl { get; set; }

            /// <inheritdoc cref="RsyaAuthorizationToken"/>
            [JsonIgnore]
            public RsyaAuthorizationToken? RsyaAuthorizationToken { get; set; }

            [JsonPropertyName("rsyaAuthorizationToken")]
            public string? RsyaAuthorizationTokenString
            {
                get => RsyaAuthorizationToken?.Token;
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                        RsyaAuthorizationToken = null;
                    else
                        RsyaAuthorizationToken = new RsyaAuthorizationToken(value);
                }
            }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public QueryValidator(
                ReportDbContext dbContext,
                IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
                RuleLevelCascadeMode = CascadeMode.Stop;
                ClassLevelCascadeMode = CascadeMode.Stop;

                RuleFor(x => x.Credentionals)
                    .SetValidator(new UserCredentionalsValidator())
                    .Must(userCredentials =>
                    {
                        var user = dbContext.Users
                            .SingleOrDefault(x => x.Email.Equals(userCredentials.Login) && x.IsActive);

                        return user is null;
                    })
                    .WithMessage("Пользователь с указанным логином уже существует")
                    .WithErrorCode("login");

                RuleFor(x => x.DeveloperPageUrl)
                    .Must(developerPageUrl =>
                    {
                        var uri = CreateUri(developerPageUrl);
                        if (uri is null)
                            return false;

                        var isValidFormat = ValidateUrlFormat(uri);
                        if (!isValidFormat)
                            return false;

                        var isAvailability = ValidateUrlAvailability(uri).Result;

                        return isAvailability;
                    })
                    .WithMessage("Указана не корректная ссылка на страницу разработчика")
                    .WithErrorCode("developerPageUrl");

                When(x => x.RsyaAuthorizationToken != null, () =>
                {
                    RuleFor(x => x.RsyaAuthorizationToken)
                        .SetValidator(new RsyaAuthorizationTokenValidator(httpClientFactory)!);
                });
            }

            private Uri? CreateUri(string developerPageUrl)
            {
                if (Uri.TryCreate(developerPageUrl, UriKind.Absolute, out var uri))
                    return uri;

                return null;
            }

            private bool ValidateUrlFormat(Uri uri)
            {
                var developerPath = "/games/developer/";
                if (!uri.AbsolutePath.StartsWith(developerPath)
                    || uri.Segments.Length < 4)
                    return false;

                var redirDataIndex = uri.Segments[3].IndexOf("#redir-data");

                string developerIdString;
                if (redirDataIndex < 0)
                    developerIdString = uri.Segments[3];
                else
                    developerIdString = uri.Segments[3][..redirDataIndex];

                var tryParseResult = int.TryParse(developerIdString, out _);

                return tryParseResult;
            }

            private async Task<bool> ValidateUrlAvailability(Uri uri)
            {
                var response = await HttpUtils.SendRequestAsync(
                    createRequest: () => new HttpRequestMessage(HttpMethod.Get, uri),
                    httpClientFactory: _httpClientFactory,
                    isEnsureSuccessStatusCode: false);

                return response.StatusCode != HttpStatusCode.NotFound;
            }
        }

        public class Handler : IRequestHandler<Query>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IHangfireBackgroundJobService _hangfireBackgroundJobService;
            private readonly IGameSyncService _gameSyncService;
            private readonly IEmailSenderService _emailSenderService;
            private readonly ConsoleLogger _logger;

            public Handler(
                ReportDbContext dbContext,
                IHangfireBackgroundJobService hangfireBackgroundJobService,
                IGameSyncService gameSyncService,
                IEmailSenderService emailSenderService,
                ILogger<CreateAccount> logger)
            {
                _dbContext = dbContext;
                _logger = new ConsoleLogger(logger);
                _hangfireBackgroundJobService = hangfireBackgroundJobService;
                _gameSyncService = gameSyncService;
                _emailSenderService = emailSenderService;
            }

            public async Task Handle(Query request, CancellationToken cancellationToken)
            {
                var existedUserWithUnConfirmedEmail = await _dbContext.Users
                    .SingleOrDefaultAsync(
                        x => x.Email.Equals(request.Credentionals.Login) && !x.IsActive,
                        cancellationToken);

                if (existedUserWithUnConfirmedEmail is not null)
                {
                    await _dbContext.Users.DeleteByKeyAsync(
                        existedUserWithUnConfirmedEmail);

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                var user = new UserModel(
                    email: request.Credentionals.Login,
                    developerPageUrl: request.DeveloperPageUrl,
                    rsyaAuthorizationToken: request.RsyaAuthorizationToken?.Token,
                    authenticationModel: null,
                    games: new HashSet<GameModel>());

                var userAuthenticationModel = new UserAuthenticationModel(
                    user: user,
                    request.Credentionals.Password);

                await _dbContext.Users
                    .AddAsync(user, cancellationToken);

                await _dbContext.UserAuthenticationModels
                    .AddAsync(userAuthenticationModel, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                //TODO: Убрать это отсюда, и сделать так, чтобы данные в аккаунт добавлялись только после подтверждения почты
                await _hangfireBackgroundJobService
                    .AddMissingHangfirejobsAsync(user, _logger, cancellationToken);

                await _gameSyncService
                    .SyncGamesAsync(user.Id, _logger, cancellationToken);

                //TODO: Убрать это отсюда после сдачи диплома, пофиксить на стороне фронта.
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