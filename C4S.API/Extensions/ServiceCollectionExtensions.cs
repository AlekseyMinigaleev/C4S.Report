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
                    //хуй пойми как, что это. Было написано с целью пофиксить спам запросам к скл серверу hangfaer`ом. не понимаю что конкретно дает эта строка.
                    // на данный момент проблема решена костыльно, hagnfire в другую бд, и в профайлере делаю фильтр.
                    QueuePollInterval = TimeSpan.FromHours(12)
                })
                .UseConsole());

            services.AddHttpContextAccessor();
            /*TODO: вроде не должно быть ошибок с nullReference проверил для неавторизоаванных пользователй ошибок нет*/
            services.AddTransient<IPrincipal>(provider => 
                provider.GetService<IHttpContextAccessor>().HttpContext.User);

            services.AddHangfireServer();
        }
    }
}