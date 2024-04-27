using AngleSharp;
using C4S.Services.Services.BackgroundJobService;
using C4S.Services.Services.CategoriesSyncService;
using C4S.Services.Services.ExcelWorksheetService;
using C4S.Services.Services.GameSyncService;
using C4S.Services.Services.GameSyncService.Helpers;
using C4S.Services.Services.GetGamesDataService;
using C4S.Services.Services.GetGamesDataService.Helpers;
using C4S.Services.Services.GetGamesDataService.RequestMethodDictionaries;
using C4S.Services.Services.JWTService;
using C4S.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.Firefox;

namespace C4S.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddTransient<IHangfireBackgroundJobService, BackgroundJobService>();
            services.AddScoped<ICategoriesSyncService, CategoriesSyncService>();
            services.AddScoped<IGameSyncService, GameSyncService>();
            services.AddScoped<IGetGameDataService, GetGameDataService>();
            services.AddScoped<GetPrivateGameDataHelper>();
            services.AddScoped<GetAppIdHelper>();
            services.AddScoped<GameModelHardCalculatedDataConverter>();
            services.AddScoped<GetPublicGameDataHelper>();
            services.AddScoped((provider) =>
            {
                var config = Configuration.Default.WithDefaultLoader();
                var browsingContext = BrowsingContext.New(config);
                return browsingContext;
            });

            services.AddScoped<IExcelWorksheetService, DetailedReportService>();
            services.AddScoped<IJwtService, JwtServise>((provider) =>
            {
                var jwtConfig = provider.GetService<IOptions<JwtConfiguration>>();

                if (jwtConfig?.Value is null)
                    throw new ArgumentNullException(nameof(IOptions<JwtConfiguration>));

                var service = new JwtServise(jwtConfig.Value);

                return service;
            });

            services.AddSingleton<IWebDriver>(provider =>
            {
                new DriverManager().SetUpDriver(new FirefoxConfig());
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.AddArgument("--headless");
                firefoxOptions.AddArgument("--lang=ru");

                var driver = new FirefoxDriver(firefoxOptions);

                return driver;
            });
        }
    }
}