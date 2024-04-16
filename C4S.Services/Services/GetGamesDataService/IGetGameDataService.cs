using C4S.DB.Models;
using C4S.Services.Services.GetGamesDataService.Models;
using C4S.Shared.Logger;

namespace C4S.Services.Services.GetGamesDataService
{
    /// <summary>
    /// Интерфейс для получения данных об играх.
    /// </summary>
    public interface IGetGameDataService
    {
        /// <summary>
        /// Получает конфиденциальные данные игры.
        /// </summary>
        /// <param name="gameModel">игра, содержащая необходимые поля для получения приватных данных</param>
        /// <param name="cancellationToken">Токен отмены задачи.</param>
        /// <returns>Объект <see cref="PrivateGameData"/>, содержащий конфиденциальные данные игры.</returns>
        public Task<PrivateGameData> GetPrivateGameDataAsync(
            GameModel gameModel,
            CancellationToken cancellationToken);

        /// <summary>
        /// Получает общедоступные данные об играх.
        /// </summary>
        /// <param name="developerPageUrl">URL страницы разработчика с информацией об играх.</param>
        /// <param name="logger">Экземпляр логгера для записи информационных сообщений.</param>
        /// <param name="cancellationToken">Токен отмены задачи.</param>
        /// <returns>Массив объектов <see cref="PublicGameData"/>, содержащих общедоступные данные об играх.</returns>
        public Task<PublicGameData[]> GetPublicGameDataAsync(
            string developerPageUrl,
            BaseLogger logger,
            CancellationToken cancellationToken);
    }
}