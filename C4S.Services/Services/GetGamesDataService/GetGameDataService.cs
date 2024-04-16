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

            var startDate = gameModel.PublicationDate;
            var endDate = DateTime.Now;
            var period = new DateTimeRange(startDate, endDate);

            var cashIncome = await _getPrivateGameDataHelper
                .GetCashIncomeAsync(
                    pageId: gameModel.PageId.Value,
                    authorization: gameModel.User.RsyaAuthorizationToken,
                    period: period,
                    cancellationToken: cancellationToken);

            var privateGameData = new PrivateGameData { CashIncome = cashIncome };

            return privateGameData;
        }

        /// <inheritdoc/>
        public async Task<PublicGameData[]> GetPublicGameDataAsync(
            string developerPageUrl,
            BaseLogger logger,
            CancellationToken cancellationToken)
        {
            var appIds = _getAppIdHelper.GetAppIdsAsync(
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