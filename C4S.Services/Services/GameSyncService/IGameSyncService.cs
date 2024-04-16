using C4S.Shared.Logger;
using Hangfire.Server;

namespace C4S.Services.Services.GameSyncService
{
    /// <summary>
    /// Сервис для синхронизации игр
    /// </summary>
    public interface IGameSyncService
    {
        /// <summary>
        /// Выполняет синхронизацию игр
        /// </summary>
        /// <param name="hangfireContext"></param>
        /// <param name="userId">Id пользователя чьи игры будут просихнронизированы</param>
        /// <param name="cancellationToken"></param>
        public Task SyncGamesAsync(
           Guid userId,
           PerformContext hangfireContext,
           CancellationToken cancellationToken);

        /// <summary>
        /// <inheritdoc cref="SyncGamesAsync(Guid, PerformContext,  CancellationToken)"/>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        public Task SyncGamesAsync(
           Guid userId,
           BaseLogger logger,
           CancellationToken cancellationToken);
    }
}