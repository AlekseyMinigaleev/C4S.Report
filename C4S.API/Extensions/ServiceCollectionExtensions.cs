using C4S.DB;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace C4S.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавляет все необходимые сервисы для хранилищ
        /// </summary>
        public static void AddStorages(this IServiceCollection services, ConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString("ReportDB");
            var hangfireConnection = configuration.GetConnectionString("HangfireDB");

            services.AddDbContext<ReportDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddHangfire(configuration => configuration
                .UseFilter(new AutomaticRetryAttribute { Attempts = 0 })
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireConnection, new SqlServerStorageOptions()
                {
                    QueuePollInterval = TimeSpan.FromHours(12)
                })
                .UseConsole());

            services.AddHttpContextAccessor();
            /*TODO: вроде не должно быть ошибок с nullReference проверил для не авторизованных пользователей ошибок нет*/
            services.AddTransient<IPrincipal>(provider => 
                provider.GetService<IHttpContextAccessor>().HttpContext.User);

            services.AddHangfireServer();
        }
    }
}