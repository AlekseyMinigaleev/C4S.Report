using C4S.DB.Models;

namespace C4S.DB.Extensions
{
    /// <summary>
    /// Справочник методов расширения для <see cref="GameModel"/>
    /// </summary>
    public static class GameExtensions
    {
        /// <summary>
        /// Получает последнюю статистику синхронизации игры.
        /// </summary>
        /// <param name="source">Множество статистик игры.</param>
        /// <returns>Последняя статистика синхронизации.</returns>
        public static GameStatisticModel GetLastSynchronizationStatistic(this ISet<GameStatisticModel> source) => source
                .OrderByDescending(x => x.LastSynchroDate)
                .First();

        /// <summary>
        /// Получает статистику синхронизации перед последней.
        /// </summary>
        /// <param name="source">Множество статистик игры.</param>
        /// <returns>Статистика синхронизации перед последней.</returns>
        public static GameStatisticModel GetBeforeLastSynchronizationStatistic(this ISet<GameStatisticModel> source) => source
                .OrderByDescending(x => x.LastSynchroDate)
                .Take(2)
                .Last();
    }
}