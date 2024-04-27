using C4S.DB;
using C4S.DB.Models;
using C4S.DB.Models.Hangfire;
using C4S.Services.Services.BackgroundJobService.Exceptions;
using C4S.Services.Services.GameSyncService;
using C4S.Shared.Extensions;
using C4S.Shared.Logger;
using Hangfire;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Principal;

namespace C4S.Services.Services.BackgroundJobService
{
    /// <inheritdoc cref="IHangfireBackgroundJobService"/>s
    public class BackgroundJobService : IHangfireBackgroundJobService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IPrincipal _principal;

        public BackgroundJobService(
            IPrincipal principal,
            ReportDbContext dbContext)
        {
            _dbContext = dbContext;
            _principal = principal;
        }

        /// <inheritdoc/>
        /// <param name="updatedJobConfig" ><inheritdoc/></param>
        public async Task UpdateRecurringJobAsync(
            HangfireJobConfigurationModel updatedJobConfig,
            CancellationToken cancellationToken = default)
        {
            var existenceJobConfig = await _dbContext.HangfireConfigurations
                .SingleAsync(x => x.JobType == updatedJobConfig.JobType, cancellationToken);

            existenceJobConfig.Update(updatedJobConfig.CronExpression, updatedJobConfig.IsEnable);

            AddOrUpdateRecurringJob(existenceJobConfig, _principal.GetUserLogin(), _principal.GetUserId());

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task AddMissingHangfirejobsAsync(
            UserModel user,
            BaseLogger logger,
            CancellationToken cancellationToken = default)
        {
            var (existingJobConfigurations,
                 allJobTypes) = await GetSourcesAsync(user.Id, cancellationToken);

            var localLogger = new AddMissingHangfireJobLogger(allJobTypes.Length, logger);

            localLogger.LogStart();

            var missingJobConfigList = GetAndProcessMissingJobConfigs(
                    user,
                    allJobTypes,
                    existingJobConfigurations,
                    localLogger);

            await _dbContext.HangfireConfigurations.AddRangeAsync(missingJobConfigList, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            localLogger.LogFinish();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task OweriteJobsAsyncs(
            BaseLogger logger,
            CancellationToken cancellationToken)
        {
            /*TODO: пока не реализовано, нужно перезаписать учитывая, что джобы привязаны к пользователям.*/
            //logger.LogInformation($"Запущен процесс перезаписи всех джоб в hangfire базе данных");
            //var jobIds = GetAllJobTypes()
            //    .Select(x => x.GetName())
            //    .ToArray();

            //using var connection = JobStorage.Current.GetConnection();
            //var recurringJobs = connection
            //    .GetRecurringJobs()
            //    .Where(x => !jobIds.Contains(x.Id))
            //    .ToList();

            //recurringJobs.ForEach(x => RecurringJob.RemoveIfExists(x.Id));
            //logger.LogInformation($"Все джобы удалены");

            //var hangfireJobConfigurations = _dbContext.HangfireConfigurations;
            //await hangfireJobConfigurations
            //    .ForEachAsync(x => AddOrUpdateRecurringJob(x, _principal.GetUserLogin()), cancellationToken);
            //logger.LogInformation($"Все джобы восстановлены");

            //logger.LogSuccess($"Процесс успешно завершен, все джобы перезаписаны");

            throw new NotImplementedException();
        }

        #region AddMissingHangfirejobsAsync Helpers

        private List<HangfireJobConfigurationModel> GetAndProcessMissingJobConfigs(
            UserModel user,
            HangfireJobType[] jobTypes,
            HangfireJobConfigurationModel[] existingJobConfigurations,
            AddMissingHangfireJobLogger localLogger)
        {
            var missingJobConfigList = new List<HangfireJobConfigurationModel>();
            foreach (var processingJobType in jobTypes)
            {
                var jobConfigOfProcessingType = existingJobConfigurations
                    .SingleOrDefault(x => x.JobType == processingJobType);

                var isMissingJobConfig = jobConfigOfProcessingType is null;
                if (isMissingJobConfig)
                {
                    var missingJobConfig = CreateMissingJobConfig(processingJobType, user);
                    missingJobConfigList.Add(missingJobConfig);
                    AddOrUpdateRecurringJob(missingJobConfig, user.Login, user.Id);
                }

                localLogger.Log(isMissingJobConfig, processingJobType.GetName());
            }

            return missingJobConfigList;
        }

        private async Task<(HangfireJobConfigurationModel[], HangfireJobType[])> GetSourcesAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            // до конца не понятно что лучше, загружать все данные в память пользователя, или множественные запросы.
            // в связи с небольшого количества джоб,  было принято решение загружать данные в память пользователя.
            var existenceJobConfigurations = await _dbContext.HangfireConfigurations
                .Where(x => x.UserId == userId)
                .ToArrayAsync(cancellationToken);

            var allJobs = GetAllJobTypes()
                .ToArray();

            return (existenceJobConfigurations, allJobs);
        }

        private HangfireJobConfigurationModel CreateMissingJobConfig(
            HangfireJobType jobType,
            UserModel user)
        {
            var missingJobConfig = new HangfireJobConfigurationModel(
                    id: Guid.NewGuid(),
                    user: user,
                    jobType: jobType,
                    cronExpression: HangfireJobConfigurationConstants.DefaultCronExpression,
                    isEnable: HangfireJobConfigurationConstants.DefaultIsEnable);

            return missingJobConfig;
        }

        private class AddMissingHangfireJobLogger
        {
            public int JobTypeCount { get; set; }
            public int ProcessedJobIndex { get; set; }
            public int CountJobWithDefaultConfig { get; set; }
            public string? LogPrefix { get; set; }

            public BaseLogger Logger { get; set; }

            public AddMissingHangfireJobLogger(int jobTypeCount, BaseLogger logger)
            {
                JobTypeCount = jobTypeCount;
                ProcessedJobIndex = 1;
                Logger = logger;
            }

            public void SetLogPrefix(string jobName)
            {
                LogPrefix = $"['{jobName}' ({ProcessedJobIndex}/{JobTypeCount})] ";
            }

            public void Log(bool isMissingJob, string jobName)
            {
                SetLogPrefix(jobName);

                if (isMissingJob)
                {
                    Logger.LogSuccess($"{LogPrefix}джоба пропущена, была выполнена регистрация со значениями по умолчанию");
                    CountJobWithDefaultConfig++;
                }
                else
                {
                    Logger.LogSuccess($"{LogPrefix}джоба уже зарегистрирована");
                }

                ProcessedJobIndex++;
            }

            public void LogStart()
            {
                Logger.LogInformation($"Запущен процесс регистрации недостающих джоб");
            }

            public void LogFinish()
            {
                Logger.LogSuccess($"Все джобы зарегистрированы. ({CountJobWithDefaultConfig}/{JobTypeCount}) со значением по умолчанию");
            }
        }

        #endregion AddMissingHangfirejobsAsync Helpers

        private IEnumerable<HangfireJobType> GetAllJobTypes()
        {
            var jobTypes = Enum
                .GetValues(typeof(HangfireJobType))
                .Cast<HangfireJobType>();

            return jobTypes;
        }

        private void AddOrUpdateRecurringJob(
            HangfireJobConfigurationModel jobConfig,
            string userLogin,
            Guid userId)
        {
            switch (jobConfig.JobType)
            {
                case HangfireJobType.SyncGameJob:
                    AddOrUpdateRecurringJob<IGameSyncService>(
                        userLogin,
                        jobConfig,
                        (service) => service.SyncGamesAsync(userId, (PerformContext)null, CancellationToken.None));
                    break;

                default:
                    break;
            }
        }

        private void AddOrUpdateRecurringJob<T>(
            string userLogin,
            HangfireJobConfigurationModel jobConfig,
            Expression<Func<T, Task>> methodCall)
        {
            try
            {
                RecurringJob.AddOrUpdate(
                     $"{userLogin} {jobConfig.JobType.GetName()}",
                    methodCall,
                    HangfireJobConfigurationConstants.DefaultCronExpression,
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    });
            }
            catch (InvalidOperationException)
            {
                var connectionString = _dbContext.Database.GetConnectionString();
                if (connectionString is null)
                    throw new HangfireDatabaseConnectException(connectionString);
                else
                    throw new HangfireDatabaseConnectException(connectionString);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}