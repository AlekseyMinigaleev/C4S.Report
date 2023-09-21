﻿using C4S.DB;
using C4S.DB.Models;
using C4S.Helpers.Logger;
using C4S.Services.Interfaces;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using S4C.YandexGateway.DeveloperPage;
using S4C.YandexGateway.DeveloperPage.Exceptions;

namespace C4S.Services.Implements
{
    /// <inheritdoc cref="IGameIdSyncService"/>
    public class GameIdSyncService : IGameIdSyncService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IDeveloperPageParser _developerPageParser;
        private BaseLogger _logger;

        public GameIdSyncService(
            ReportDbContext dbContext,
            IDeveloperPageParser developerPageParser)
        {
            _dbContext = dbContext;
            _developerPageParser = developerPageParser;
        }

        /// <inheritdoc/>
        public async Task SyncAllGameIdAsync(
            PerformContext hangfireContext,
            CancellationToken cancellationToken = default)
        {
            _logger = new HangfireLogger(hangfireContext);

            var finalLogMessage = "Процесс успешно завершен.";
            var errorLogMessage = "Процесс завершен c ошибкой: ";
            var warningLogMessage = "Процесс завершен: ";
            var logLevel = LogLevel.Success;
            try
            {
                _logger.LogInformation($"Запущен процесс парсинга id игр:");
                await RunAsync(cancellationToken);
            }
            catch (EmptyDeveloperPageException e)
            {
                finalLogMessage = $"{warningLogMessage}{e.Message}";
                logLevel = LogLevel.Warning;
                throw;
            }
            catch (Exception e)
            {
                finalLogMessage = $"{errorLogMessage}{e.Message}";
                logLevel = LogLevel.Error;
                throw;
            }
            finally
            {
                _logger.Log(finalLogMessage, logLevel);
            }
        }

        private async Task RunAsync(
            CancellationToken cancellationToken)
        {
            /*TODO: исправить полсе добавления авторизации*/
            var developerPageUrl = _dbContext.Users
                .First()
                .DeveloperPageUrl;

            _logger.LogInformation($"Получение id всех игр.");
            var gameIds = await _developerPageParser
                .GetGameIdsAsync(developerPageUrl, _logger, cancellationToken);

            _logger.LogInformation($"Начало обработки всех id:");
            await ProcessingIncomingGameIdsAsync(gameIds, cancellationToken);
            _logger.LogSuccess($"Все id обработаны.");
        }

        private async Task ProcessingIncomingGameIdsAsync(
            int[] gameIds,
            CancellationToken cancellationToken)
        {
            var countOfMissingGames = 0;
            foreach (var gameId in gameIds)
            {
                var increment = await ProcessingIncomingIdAsync(gameId, cancellationToken);
                countOfMissingGames += increment;
            }
            _logger.LogInformation($"Начло добавления {countOfMissingGames}, записей в базу данных:");
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogSuccess($"Все записи успешно добавлены");
        }

        // этот метод возвращает инкремент для переменной countOfMissingGame
        private async Task<int> ProcessingIncomingIdAsync(
            int gameId,
            CancellationToken cancellationToken)
        {
            var existingGameIds = _dbContext.Games
                .Select(x => x.Id);

            var isExistingGame = await existingGameIds
                .ContainsAsync(gameId, cancellationToken);

            if (!isExistingGame)
            {
                /*TODO: исправить после добавления авторизации*/
                _logger.LogInformation($"[{gameId}] id игры не содержится в базе данных.");
                var user = _dbContext.Users
                .First();

                var gameModel = new GameModel(gameId, user!);

                await _dbContext.Games
                    .AddAsync(gameModel, cancellationToken);
                _logger.LogInformation($"[{gameId}] id игры помечено на добавление в базу данных.");

                // если игра не содержится в бд, увеличиваем countOfMissingGame
                return 1;
            }
            else
            {
                _logger.LogInformation($"[{gameId}] id игры уже содержится в базе данных.");

                // если игра содержится в бд, не увеличиваем countOfMissingGame
                return 0;
            }
        }
    }
}