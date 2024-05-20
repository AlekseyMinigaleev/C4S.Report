using C4S.Services.Exceptions;
using C4S.Services.Services.GetGamesDataService.Enums;
using C4S.Services.Services.GetGamesDataService.Models;
using C4S.Shared.Extensions;
using C4S.Shared.Logger;
using C4S.Shared.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace C4S.Services.Services.GetGamesDataService.RequestMethodDictionaries
{
    /// <summary>
    /// Вспомогательный класс для получения <see cref="PublicGameData"/> игр
    /// </summary>
    public class GetPublicGameDataHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _yandexGetGameRequestURL;
        private const string PreviewFormat = "pjpg350x209";

        public GetPublicGameDataHelper(
            IHttpClientFactory httpClient,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClient;
            _yandexGetGameRequestURL = configuration["YandexGetGame"]!;

            ArgumentException.ThrowIfNullOrEmpty(
                _yandexGetGameRequestURL,
                "в файле appsetting.json не указана или указана неверно ссылка на запрос по Яндекс играм");
        }

        /// <summary>
        /// Получает информацию об играх по их идентификаторам.
        /// </summary>
        /// <param name="appIds">Массив идентификаторов игр.</param>
        /// <param name="logger">Экземпляр логгера для записи информационных сообщений.</param>
        /// <param name="cancellationToken">Токен отмены задачи.</param>
        /// <returns>Массив объектов <see cref="PublicGameData"/>.</returns>
        public async Task<PublicGameData[]> GetGamesInfoAsync(
            int[] appIds,
            BaseLogger logger,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation($"Получение всех данных по всем играм");

            var result = new List<PublicGameData>();
            foreach (var appId in appIds)
            {
                logger.Prefix = $"[{appId}]";
                logger.LogInformation($"Составление запроса на сервер Яндекс");
                HttpRequestMessage createRequest() =>
                    YGApiHttpRequestMethodDictionary.GetGamesInfo(
                        _yandexGetGameRequestURL, /*TODO: передавать внутрь только endpoint, внутри захардкодить BaseURL*/
                        appId,
                        RequestFormat.Long);

                var httpResponseMessage = await HttpUtils.SendRequestAsync(
                    createRequest: createRequest,
                    httpClientFactory: _httpClientFactory,
                    cancellationToken: cancellationToken);
                logger.LogSuccess($"Ответ от Яндекса успешно получен");

                logger.LogInformation($"Начало обработки ответа");
                var gameInfoModel = await DeserializeObjectsAsync(
                    httpResponseMessage,
                    logger,
                    cancellationToken);
                logger.LogSuccess($"Ответ успешно обработан");

                result.Add(gameInfoModel);
            }
            logger.Prefix = default;

            logger.LogSuccess($"Процесс получения данных по всем играм успешно завершен");
            return result.ToArray();
        }

        private async Task<PublicGameData> DeserializeObjectsAsync(
            HttpResponseMessage httpResponseMessage,
            BaseLogger logger,
            CancellationToken cancellationToken)
        {
            var jsonString = await httpResponseMessage.Content
                .ReadAsStringAsync(cancellationToken);

            var jObject = JObject.Parse(jsonString);
            var gameJToken = jObject.GetValue("game") ??
                throw new ArgumentNullException("Не удалось получить объект game из ответа Яндекса");

            var title = gameJToken.GetValue<string>("title");
            var appId = gameJToken.GetValue<int>("appID");
            var firstPublished = gameJToken.GetValue<int>("firstPublished");
            var evaluation = gameJToken.GetValue<double>("rating");
            var categoriesNames = gameJToken.GetValue<string[]>("categoriesNames");
            var rating = gameJToken.GetValue<int?>("gqRating");
            var previewURL = gameJToken.GetValue<string>("media", "cover", "prefix-url");

            ProcessNullableField(
                field: title,
                key: "title",
                log: () => logger.LogError($"Не удалось получить название"),
                withException: true);

            ProcessNullableField(
                field: previewURL,
                key: "media, cover, prefix-url",
                log: () => logger.LogError($"Не удалось получить превью"),
                withException: true);

            ProcessNullableField(
                field: categoriesNames,
                key: "categoriesNames",
                log: () => logger.LogError($"Не удалось получить категории"),
                withException: true);

            ProcessNullableField(
                field: rating,
                key: "gqRating",
                log: () => logger.LogError($"Не удалось получить рейтинг"),
                withException: false);

            var gameInfo = new PublicGameData(
                title: title!,
                appId: appId,
                firstPublished: firstPublished,
                evaluation: evaluation,
                categoriesNames: categoriesNames!,
                previewURL: $"{previewURL!}{PreviewFormat}",
                rating: rating);

            return gameInfo;
        }

        private static void ProcessNullableField<T>(
            T field,
            string key,
            Action log,
            bool withException)
        {
            if (field is null)
            {
                log();

                if (withException)
                    throw new JsonPropertyNullValueException(key);
            }
        }
    }
}