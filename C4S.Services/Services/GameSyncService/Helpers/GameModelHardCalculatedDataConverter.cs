using C4S.DB;
using C4S.DB.Models;
using C4S.DB.ValueObjects;
using C4S.Services.Services.GetGamesDataService;
using Microsoft.EntityFrameworkCore;

namespace C4S.Services.Services.GameSyncService.Helpers
{
    /// <summary>
    /// Класс, выполняющий преобразование публичных игровых данных в сложно высчитываемые данные,
    /// такие как категории, рейтинг и денежные поступления.
    /// </summary>
    public class GameModelHardCalculatedDataConverter
    {
        private readonly ReportDbContext _dbContext;

        public GameModelHardCalculatedDataConverter(
            ReportDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Преобразует категории из их имен в соответствующие модели категорий.
        /// </summary>
        /// <param name="categoriesNames">Имена категорий для преобразования.</param>
        /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
        /// <returns>Коллекция моделей категорий.</returns>
        public async Task<IEnumerable<CategoryModel>> ConvertCategories(
            IEnumerable<string> categoriesNames,
            CancellationToken cancellationToken)
        {
            var categories = await _dbContext.Categories
                .Where(x => categoriesNames.Contains(x.Name))
                .ToListAsync(cancellationToken);

            return categories;
        }

        /// <summary>
        /// Преобразует данные о доходе с учетом предыдущих статистик игры.
        /// </summary>
        /// <param name="sourceCashIncome">Исходное значение дохода.</param>
        /// <param name="gameStatistics">Статистика игры.</param>
        /// <returns>Объект <see cref="ValueWithProgress{T}"/> с данными о денежных поступлениях.</returns>
        public ValueWithProgress<double>? ConvertCashIncome(
            double? sourceCashIncome,
            IEnumerable<GameStatisticModel>? gameStatistics)
        {
            if (gameStatistics is null)
                return null;

            var lastSynchroGameStatisticWithCashIncome = gameStatistics
                .Where(x => x.CashIncome is not null)
                .OrderByDescending(x => x.LastSynchroDate)
                .FirstOrDefault();

            if (lastSynchroGameStatisticWithCashIncome is null)
            {
                if (sourceCashIncome.HasValue)
                    return new ValueWithProgress<double>(
                        sourceCashIncome.Value,
                        sourceCashIncome.Value);
                else
                    return null;
            }
            else
            {
                var lastActualValue = lastSynchroGameStatisticWithCashIncome.CashIncome!.ActualValue;

                if (sourceCashIncome.HasValue)
                    return new ValueWithProgress<double>(
                        sourceCashIncome.Value,
                        sourceCashIncome.Value - lastActualValue);
                else
                    return new ValueWithProgress<double>(
                        0,
                        0 - lastActualValue);
            }
        }

        /// <summary>
        /// Преобразует данные о рейтинге с учетом предыдущих статистик игры.
        /// </summary>
        /// <param name="existGameModel">Существующая модель игры.</param>
        /// <param name="sourceRating">Исходное значение рейтинга.</param>
        /// <returns>Объект <see cref="ValueWithProgress{T}"/> с данными о рейтинге.</returns>
        public ValueWithProgress<int>? ConvertRating(
            GameModel? existGameModel,
            int? sourceRating)
        {
            if (!sourceRating.HasValue)
                return null;

            if (existGameModel is not null)
            {
                var lastSynchroGameStatisticWithRating = existGameModel.GameStatistics
                    .Where(x => x.Rating is not null)
                    .OrderByDescending(x => x.LastSynchroDate)
                    .FirstOrDefault();

                if (lastSynchroGameStatisticWithRating is not null)
                    return new ValueWithProgress<int>(
                        sourceRating.Value,
                        sourceRating.Value - lastSynchroGameStatisticWithRating.Rating!.ActualValue);
                else
                    return new ValueWithProgress<int>(
                       sourceRating.Value,
                       sourceRating.Value);
            }
            else
            {
                return new ValueWithProgress<int>(
                    sourceRating.Value,
                    sourceRating.Value);
            }
        }
    }
}