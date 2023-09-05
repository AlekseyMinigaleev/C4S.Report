﻿using C4S.Helpers.Logger;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using S4C.YandexGateway.DeveloperPage.Enums;
using S4C.YandexGateway.DeveloperPage.Exceptions;
using S4C.YandexGateway.DeveloperPage.Models;

namespace S4C.YandexGateway.DeveloperPage
{
    /// <inheritdoc cref="IDeveloperPageGetaway"/>
    public class DeveloperPageGateway : IDeveloperPageGetaway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _yandexGamesRequestUrl;

        public DeveloperPageGateway(
            IHttpClientFactory httpClient,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClient;
            _yandexGamesRequestUrl = configuration["YandexGamesRequestUrl"]!;
            ArgumentException.ThrowIfNullOrEmpty("в файле appsetting.json не указана или указана неверно ссылка на запрос по Яндекс играм");
        }

        /// <inheritdoc/>
        public async Task<GameInfoModel[]> GetGameInfoAsync(
            int[] gameIds,
            BaseLogger logger,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation($"Составление запроса на сервер Яндекс");
            var httpResponseMessage = await SendRequestAsync(() =>
                HttpRequestMethodDitctionary.GetGamesInfo(
                    _yandexGamesRequestUrl,
                    gameIds,
                    RequestFormat.Long),
                cancellationToken);
            logger.LogSuccess($"Ответ от Яндекса успешно получен");

            logger.LogInformation($"Начало обработки ответа");
            var gameViewModels = await DeserializeObjectsAsync(
                httpResponseMessage,
                cancellationToken);
            logger.LogSuccess($"Ответ успешно обработан");

            return gameViewModels;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(
            Func<HttpRequestMessage> createRequest,
            CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient();

            var request = createRequest();
            var response = await client
                .SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        private async Task<GameInfoModel[]> DeserializeObjectsAsync(
            HttpResponseMessage httpResponseMessage,
            CancellationToken cancellationToken)
        {
            var jsonString = await httpResponseMessage.Content
                .ReadAsStringAsync(cancellationToken);
            var gamesJToken = GetGamesJToken(jsonString);

            var results = new GameInfoModel[gamesJToken.Count];
            for (int i = 0; i < gamesJToken.Count; i++)
            {
                var title = GetValue<string>("title", gamesJToken[i], jsonString);
                var appId = GetValue<int>("appID", gamesJToken[i], jsonString);
                var firstPublished = GetValue<int>("firstPublished", gamesJToken[i], jsonString);
                var rating = GetValue<double>("rating", gamesJToken[i], jsonString);
                var playersCount = GetValue<int>("playersCount", gamesJToken[i], jsonString);
                var categoriesNames = GetValue<string[]>("categoriesNames", gamesJToken[i], jsonString);

                var gameDataViewModel = new GameInfoModel(
                    title: title,
                    appId: appId,
                    firstPublished: firstPublished,
                    rating: rating,
                    playersCount: playersCount,
                    categoriesNames: categoriesNames);

                results[i] = gameDataViewModel;
            }

            return results;
        }

        private static JArray GetGamesJToken(string jsonString)
        {
            var responseJObject = JObject.Parse(jsonString);
            var gamesJToken = responseJObject["games"] as JArray
                ?? throw new InvalidContractException(jsonString, "games");

            return gamesJToken;
        }

        private static T GetValue<T>(string key, JToken jToken, string jsonString)
        {
            T value;
            if (typeof(T).IsArray)
                value = jToken[key]!.ToObject<T>()!;
            else
                value = jToken[key]!.Value<T>()!;
            var result = value ?? throw new InvalidContractException(jsonString, key);
            return result;
        }
    }
}