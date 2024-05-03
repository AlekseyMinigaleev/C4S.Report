using AutoMapper;
using C4S.DB;
using C4S.DB.Models;
using C4S.DB.ValueObjects;
using C4S.Services.Services.GameSyncService.Helpers;
using C4S.Services.Services.GetGamesDataService;
using C4S.Services.Services.GetGamesDataService.Models;
using C4S.Shared.Extensions;
using C4S.Shared.Logger;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;

namespace C4S.Services.Services.GameSyncService
{
    /// <inheritdoc cref="IGameSyncService"/>
    public class GameSyncService : IGameSyncService
    {
        private readonly ReportDbContext _dbContext;
        private readonly GameModelHardCalculatedDataConverter _gameModelHardCalculatedDataMapper;
        private readonly IGetGameDataService _getGameDataService;
        private readonly IMapper _mapper;

        private BaseLogger _logger;
        private UserModel _user;
        private IQueryable<GameModel> _existingGameModelsQuery;

        public GameSyncService(
            ReportDbContext dbContext,
            GameModelHardCalculatedDataConverter gameModelHardCalculatedDataMapper,
            IGetGameDataService getGameDataService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _gameModelHardCalculatedDataMapper = gameModelHardCalculatedDataMapper;
            _getGameDataService = getGameDataService;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task SyncGamesAsync(
            Guid userId,
            PerformContext hangfireContext,
            CancellationToken cancellationToken)
        {
            var hangfireLogger = new HangfireLogger(hangfireContext);

            await SyncGamesAsync(
                userId,
                hangfireLogger,
                cancellationToken);
        }

        /// <inheritdoc/>
        public async Task SyncGamesAsync(
            Guid userId,
            BaseLogger logger,
            CancellationToken cancellationToken)
        {
            _logger = logger;
            _user = GetUser(userId);
            _existingGameModelsQuery = _dbContext.Games
                .Where(x => x.UserId == _user.Id);

            _logger.LogInformation("Начат процесс получения данных по всем играм");
            var newGameModels = await GetGameModelToSynchroAsync(cancellationToken);
            _logger.LogSuccess("процесс получения данных по всем играм завершен");

            _logger.LogInformation("Сохранение данных в бд");
            await UpdateDatabaseAsync(newGameModels, cancellationToken);
            _logger.LogSuccess("Данные успешно сохранены");
        }

        private async Task<IEnumerable<GameModel>> GetGameModelToSynchroAsync(CancellationToken cancellationToken)
        {
            var publicGamesData = await _getGameDataService.GetPublicGameDataAsync(
               _user.DeveloperPageUrl,
                _logger,
               cancellationToken);

            _logger.LogInformation("Обработка полученных данных");

            var newGameModels = new List<GameModel>();
            foreach (var publicGameData in publicGamesData)
            {
                var newGameModel = _mapper.Map<PublicGameData, GameModel>(publicGameData);
                var newGameStatistic = _mapper.Map<PublicGameData, GameStatisticModel>(publicGameData);

                /*TODO: множественное обращение к бд*/
                await EnrichByPageId(newGameModel, cancellationToken);

                newGameModel.GameStatistics = new HashSet<GameStatisticModel>() { newGameStatistic };
                newGameModel.SetUser(_user);

                await EnrichByHardCalculatedDataAsync(
                    publicGameData: publicGameData,
                    gameModelToEnrich: newGameModel,
                    cancellationToken: cancellationToken);

                newGameModels.Add(newGameModel);
            }
            _logger.LogSuccess("Данные успешно обработаны");

            return newGameModels;
        }

        private async Task<GameModel> EnrichByPageId(GameModel gameModelToEnrich, CancellationToken cancellationToken)
        {
            var existingGameModel = await _dbContext.Games
                   .Where(x => x.UserId == _user.Id)
                   .SingleOrDefaultAsync(x => x.AppId == gameModelToEnrich.AppId, cancellationToken);
            gameModelToEnrich.SetPageId(existingGameModel?.PageId);

            return gameModelToEnrich;
        }

        private async Task EnrichByHardCalculatedDataAsync(
            PublicGameData publicGameData,
            GameModel gameModelToEnrich,
            CancellationToken cancellationToken)
        {
            var existGameModel = _existingGameModelsQuery
                .Include(x => x.GameStatistics)
                .Include(x => x.User)
                .SingleOrDefault(x => x.AppId == gameModelToEnrich.AppId);

            var privateGameData = await _getGameDataService
                .GetPrivateGameDataAsync(existGameModel, cancellationToken);

            var categories = await _gameModelHardCalculatedDataMapper
                .ConvertCategories(publicGameData.CategoriesNames, cancellationToken);

            var rating = _gameModelHardCalculatedDataMapper
                .ConvertRating(existGameModel, publicGameData.Rating);

            ValueWithProgress<double>? cashIncome;
            if (gameModelToEnrich.PageId is null)
                cashIncome = null;
            else
                cashIncome = _gameModelHardCalculatedDataMapper
                         .ConvertCashIncome(privateGameData.CashIncome, existGameModel?.GameStatistics);

            gameModelToEnrich.AddCategories(categories);
            gameModelToEnrich.GameStatistics.First().Rating = rating;
            gameModelToEnrich.GameStatistics.First().CashIncome = cashIncome;
        }

        private UserModel GetUser(Guid userId)
        {
            var user = _dbContext.Users
                .SingleOrDefault(x => x.Id == userId);

            if (user is null)
                throw new ArgumentNullException($"{nameof(user)} Пользователь с указанным Id не был найден.");

            return user;
        }

        private async Task UpdateDatabaseAsync(IEnumerable<GameModel> newGameModels, CancellationToken cancellationToken)
        {
            var existingGameModels = _existingGameModelsQuery
                .Include(x => x.CategoryGameModels)
                    .ThenInclude(x => x.Category)
                .ToList();

            SetIsArchive(existingGameModels, newGameModels);

            Add(existingGameModels, newGameModels);

            Update(existingGameModels, newGameModels);

            _logger.LogInformation($"Начало обновление бд");
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogSuccess($"Бд обновлена");
        }

        private void SetIsArchive(
                IEnumerable<GameModel> existingGameModels,
                IEnumerable<GameModel> newGameModels)
        {
            Archive();
            UnArchie();

            void Archive()
            {
                var gameModelsToArchive = existingGameModels
                    .GetItemsNotInSecondCollection(newGameModels)
                    .Where(x => x.IsArchived != true)
                    .ToList();

                gameModelsToArchive
                    .ForEach(x => x.SetIsArchived(true));

                _logger.LogInformation($"Пометка 'архивные' установлена {gameModelsToArchive.Count} играм");
            }

            void UnArchie()
            {
                var gameModelsToUnArchive = existingGameModels
                    .Where(
                        x => newGameModels
                            .Select(x => x.AppId)
                            .Contains(x.AppId)
                        && x.IsArchived == true)
                    .ToList();

                gameModelsToUnArchive
                    .ForEach(x => x.SetIsArchived(false));

                _logger.LogInformation($"Пометка 'архивные' снята {gameModelsToUnArchive.Count} играм");
            }
        }

        private void Add(
                IEnumerable<GameModel> existingGameModels,
                IEnumerable<GameModel> newGameModels)
        {
            var gameModelsToAdd = newGameModels
               .GetItemsNotInSecondCollection(existingGameModels);

            _dbContext.Games.AddRange(gameModelsToAdd);
            _logger.LogInformation($"На добавление помечено {gameModelsToAdd.Count()} игр");
        }

        private void Update(
            IEnumerable<GameModel> existingGameModels,
            IEnumerable<GameModel> newGameModels)
        {
            var count = 0;
            foreach (var newGameModel in newGameModels)
            {
                var modelForUpdate = existingGameModels
                    .SingleOrDefault(x => x.AppId == newGameModel.AppId);

                if (modelForUpdate is not null)
                {
                    modelForUpdate.Update(
                        name: newGameModel.Name,
                        publicationDate: newGameModel.PublicationDate,
                        previewURL: newGameModel.PreviewURL,
                        categories: newGameModel.Categories
                        );

                    var gameStatistic = newGameModel.GameStatistics.First();
                    gameStatistic.GameId = modelForUpdate.Id;

                    _dbContext.GamesStatistics
                        .Add(gameStatistic);
                    count++;
                }
            }

            _logger.LogInformation($"На обновление помечено {count} игр");
        }
    }
}