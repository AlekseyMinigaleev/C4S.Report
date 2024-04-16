using C4S.Shared.Logger;

namespace C4S.Services.Services.CategoriesSyncService
{
    /// <summary>
    /// Сервис для синхронизации статусов игр
    /// </summary>
    public interface ICategoriesSyncService
    {
        /// <summary>
        /// Выполняет синхронизацию статусов игр с яндкс играми
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        public Task SyncCategoriesAsync(BaseLogger logger, CancellationToken cancellationToken);
    }
}
