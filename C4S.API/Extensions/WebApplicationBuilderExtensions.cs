using C4S.Shared.Logger;
using Microsoft.EntityFrameworkCore;

namespace C4S.API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// <inheritdoc cref="WebApplicationBuilder.Build()"/>
        /// </summary>
        /// <remarks>
        /// Перед созданием <see cref="WebApplication"/> создает хранилище для Hangfire
        /// </remarks>
        /// <returns><inheritdoc cref="WebApplicationBuilder.Build()"/></returns>
        public static WebApplication BuildWithHangfireStorage(
            this WebApplicationBuilder builder,
            ConfigurationManager configuration)
        {
            CreateHangfireStorage(configuration);
            var app = builder.Build();
            return app;
        }

        private static void CreateHangfireStorage(ConfigurationManager configuration)
        {
            var logger = CreateLogger();
            var options = CreateDbContextOptions(configuration);

            using var dbContext = new DbContext(options);
            logger.LogInformation("Начат процесс создания бд для Hangfire");
            var isCreated = dbContext.Database.EnsureCreated();
            var loggerMessage = isCreated
                ? "База данных создана"
                : "База данных уже существует";

            logger.LogSuccess(loggerMessage);
        }

        private static ConsoleLogger CreateLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var defaultLogger = loggerFactory.CreateLogger<Program>();
            var consoleLogger = new ConsoleLogger(defaultLogger);
            return consoleLogger;
        }

        private static DbContextOptions<DbContext> CreateDbContextOptions(ConfigurationManager configuration)
        {
            var hangfireConnectionString = configuration.GetConnectionString("HangfireDB");
            var options = new DbContextOptionsBuilder<DbContext>()
                .UseSqlServer(hangfireConnectionString)
                .Options;
            return options;
        }
    }
}