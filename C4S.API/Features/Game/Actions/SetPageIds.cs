using C4S.DB;
using C4S.Services.Services.GetGamesDataService.Helpers;
using C4S.Shared.Extensions;
using C4S.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Principal;
using C4S.API.Extensions;

namespace C4S.API.Features.Game.Actions
{
    public class SetPageIds
    {
        public class Command : IRequest<ViewModel[]>
        {
            public Body[] Body { get; set; }
        }

        public class Body
        {
            /// <summary>
            /// Id игры
            /// </summary>
            public Guid GameId { get; set; }

            /// <summary>
            /// Id страницы
            /// </summary>
            /// <remarks>
            /// Поле для взаимодействия с РСЯ
            /// </remarks>
            public long? PageId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(
                IPrincipal principal,
                ReportDbContext dbContext)
            {
                ClassLevelCascadeMode = CascadeMode.Stop;

                RuleFor(x => x)
                   .Must(x =>
                   {
                       var rsyaAuthorizationToken = principal.GetUserRsyaAuthorizationToken();
                       return rsyaAuthorizationToken is not null;
                   })
                   .WithErrorCode(HttpStatusCode.Unauthorized.ToString())
                   .WithMessage("Для использования возможности сбора статистики по прибыли игр, " +
                    "необходимо указать токен авторизации РСЯ");

                RuleForEach(x => x.Body)
                    .SetValidator(new BodyValidator(principal, dbContext));
            }
        }

        public class BodyValidator : AbstractValidator<Body>
        {
            public BodyValidator(
                IPrincipal principal,
                ReportDbContext dbContext)
            {
                RuleFor(x => x.GameId)
                    .MustAsync(async (gameId, cancellationToken) =>
                    {
                        var userId = principal.GetUserId();

                        var result = await dbContext.Games
                            .Where(x => x.UserId == userId)
                            .Select(x => x.Id)
                            .ContainsAsync(gameId, cancellationToken);

                        return result;
                    })
                    .WithMessage(x => $"{x.GameId} was not found")
                    .WithErrorCode("404");

                RuleFor(x => x.PageId)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage(x => $"{x.GameId} pageId cannot have a value less than 0 ");
            }
        }

        public class ViewModel
        {
            /// <summary>
            /// Id игры
            /// </summary>
            public Guid GameId { get; set; }

            /// <summary>
            /// Id страницы
            /// </summary>
            /// <remarks>
            /// Поле для взаимодействия с РСЯ
            /// </remarks>
            public long? PageId { get; set; }

            /// <summary>
            /// Флаг показывающий было ли установлено значение
            /// </summary>
            public bool IsSuccessfullySet { get; set; }
        }

        public class Handler : IRequestHandler<Command, ViewModel[]>
        {
            private readonly ReportDbContext _dbContext;
            private readonly GetPrivateGameDataHelper _getPrivateGameDataHelper;
            private readonly IPrincipal _principal;

            public Handler(
                ReportDbContext dbContext,
                GetPrivateGameDataHelper getPrivateGameDataHelper,
                IPrincipal principal)
            {
                _dbContext = dbContext;
                _getPrivateGameDataHelper = getPrivateGameDataHelper;
                _principal = principal;
            }

            public async Task<ViewModel[]> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                var rsyaAuthorizationToken =
                    _principal.GetUserRsyaAuthorizationToken();

                var response = new List<ViewModel>();
                foreach (var body in request.Body)
                {
                    var isSuccessfullySet =
                        await TrySetPageId(body, rsyaAuthorizationToken!, cancellationToken);

                    var viewModel = new ViewModel
                    {
                        GameId = body.GameId,
                        PageId = body.PageId,
                        IsSuccessfullySet = isSuccessfullySet
                    };

                    response.Add(viewModel);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                return [.. response];
            }

            private async Task<bool> TrySetPageId(
                Body body,
                string rsyaAuthorizationToken,
                CancellationToken cancellationToken)
            {
                bool isSuccessfullySet = false;

                if (body.PageId.HasValue)
                {
                    var period = new DateTimeRange(DateTime.Now, DateTime.Now);
                    var result = await _getPrivateGameDataHelper
                        .GetCashIncomeAsync(
                               pageId: body.PageId.Value,
                               authorization: rsyaAuthorizationToken,
                               period: period,
                               cancellationToken: cancellationToken);
                    isSuccessfullySet = result.HasValue;
                }
                else
                {
                    isSuccessfullySet = true;
                }

                if (isSuccessfullySet)
                    (await _dbContext.Games
                        .SingleAsync(x => x.Id == body.GameId, cancellationToken))
                        .SetPageId(body.PageId.HasValue ? body.PageId.Value : null);

                return isSuccessfullySet;
            }
        }
    }
}