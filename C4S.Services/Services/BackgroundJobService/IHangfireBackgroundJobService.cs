using C4S.DB.Models;
using C4S.DB.Models.Hangfire;
using C4S.Shared.Logger;

namespace C4S.Services.Services.BackgroundJobService
{
    /// <summary>
    /// Сервис для управления Hangfires job
    /// </summary>
    public interface IHangfireBackgroundJobService
    {
        /// <summary>
        /// Выполняет создание недостающих джоб.
        /// </summary>
        /// <remarks>
        /// Работа метода включает в себя обновление таблицы <see cref="HangfireJobConfigurationModel"/>
        /// </remarks>
        public Task AddMissingHangfirejobsAsync(
            UserModel user,
            BaseLogger logger,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Выполняет обновление указанной джобы.
        /// </summary>
        /// <remarks>
        /// Работа метода включает в себя обновление таблицы <see cref="HangfireJobConfigurationModel"/>
        /// </remarks>
        /// <param name="jobConfig">джоба которую нужно обновить</param>
        public Task UpdateRecurringJobAsync(
            UpdateHangfireJobConfigurationDTO jobConfig,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// выполнят перезапись всех джоб в hangfire базе данных
        /// </summary>
        /// <remarks>
        /// Перезаписанные джобы будут с конфигурацией, сохраненной в Report базе данных
        /// </remarks>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        public Task OweriteJobsAsyncs(
            BaseLogger logger,
            CancellationToken cancellationToken);
    }
}