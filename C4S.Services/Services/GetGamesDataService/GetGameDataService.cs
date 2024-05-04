using C4S.DB.Models;
using C4S.Services.Services.GetGamesDataService.Helpers;
using C4S.Services.Services.GetGamesDataService.Models;
using C4S.Services.Services.GetGamesDataService.RequestMethodDictionaries;
using C4S.Shared.Logger;
using C4S.Shared.Models;

namespace C4S.Services.Services.GetGamesDataService
{
    /// <inheritdoc cref="IGetGameDataService"/>
    public class GetGameDataService : IGetGameDataService
    {
        private readonly GetAppIdHelper _getAppIdHelper;
        private readonly GetPublicGameDataHelper _getPublicGameDataHelper;
        private readonly GetPrivateGameDataHelper _getPrivateGameDataHelper;

        private const int MAX_CASH_INCOME_REPORT_DAYS = (2 * 365) + 1; // 2 года 1 день

        public GetGameDataService(
            GetAppIdHelper getAppIdHelper,
            GetPublicGameDataHelper getPublicGameDataHelper,
            GetPrivateGameDataHelper getPrivateGameDataHelper)
        {
            _getAppIdHelper = getAppIdHelper;
            _getPublicGameDataHelper = getPublicGameDataHelper;
            _getPrivateGameDataHelper = getPrivateGameDataHelper;
        }

        /// <inheritdoc/>
        public async Task<PrivateGameData> GetPrivateGameDataAsync(
             GameModel? gameModel,
             CancellationToken cancellationToken)
        {
            if (gameModel?.User.RsyaAuthorizationToken is null || !gameModel.PageId.HasValue)
                return new PrivateGameData();

            var period = new DateTimeRange(gameModel.PublicationDate, DateTime.Now);

            if (period.Difference.TotalDays <= MAX_CASH_INCOME_REPORT_DAYS)
            {
                var cashIncome = 
                    await GetCashIncomeAsync(
                        gameModel,
                        period,
                        cancellationToken);

                return new PrivateGameData { CashIncome = cashIncome };
            }
            else
            {
                var cashIncome =
                    await GetCashIncomeForLongPeriodAsync(
                        gameModel,
                        period,
                        cancellationToken);

                return new PrivateGameData { CashIncome = cashIncome };
            }
        }

        private async Task<double?> GetCashIncomeAsync(
            GameModel gameModel,
            DateTimeRange period,
            CancellationToken cancellationToken)
        {
            var cashIncome = await _getPrivateGameDataHelper
                .GetCashIncomeAsync(
                    pageId: gameModel.PageId!.Value,
                    authorization: gameModel.User.RsyaAuthorizationToken!,
                    period: period,
                    cancellationToken: cancellationToken);

            return cashIncome;
        }

        private async Task<double?> GetCashIncomeForLongPeriodAsync(
            GameModel gameModel,
            DateTimeRange period,
            CancellationToken cancellationToken)
        {
            var numPeriods = (int)Math
                .Ceiling(period.Difference.TotalDays / MAX_CASH_INCOME_REPORT_DAYS);

            var periodLength = (int)Math
                .Floor(period.Difference.TotalDays / numPeriods);

            double? cashIncome = null;

            for (int i = 0; i < numPeriods; i++)
            {
                var periodStartDate = period.StartDate
                    .AddDays(i * periodLength);

                var periodEndDate = (i == numPeriods - 1)
                    ? period.FinishDate
                    : periodStartDate.AddDays(periodLength - 1);

                var newPeriod = new DateTimeRange(periodStartDate, periodEndDate);

                var periodCashIncome =
                    await GetCashIncomeAsync(
                        gameModel,
                        newPeriod,
                        cancellationToken);

                cashIncome = cashIncome.HasValue
                    ? cashIncome + periodCashIncome
                    : periodCashIncome;
            }

            return cashIncome;
        }

        /// <inheritdoc/>
        public async Task<PublicGameData[]> GetPublicGameDataAsync(
            string developerPageUrl,
            BaseLogger logger,
            CancellationToken cancellationToken)
        {
            var appIds = _getAppIdHelper
                .GetAppIdsAsync(
                    developerPageUrl,
                    logger);

            var publicGameData = await _getPublicGameDataHelper
                .GetGamesInfoAsync(
                    appIds,
                    logger,
                    cancellationToken);

            return publicGameData;
        }
    }
}