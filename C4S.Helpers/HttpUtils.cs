﻿namespace S4C.Helpers
{
    /// <summary>
    /// Класс, предоставляющий вспомогательные методы для выполнения HTTP-запросов
    /// </summary>
    public static class HttpUtils
    {
        /// <summary>
        /// Асинхронно выполняет HTTP-запрос, созданный с помощью делегата, переданного в параметре <paramref name="createRequest"/>
        /// </summary>
        /// <param name="createRequest">Делегат, возвращающий HTTP-запрос, который будет отправлен</param>
        /// <returns>Объект <see cref="HttpResponseMessage"/>, представляющий ответ на HTTP-запрос</returns>
        public static async Task<HttpResponseMessage> SendRequestAsync(
            Func<HttpRequestMessage> createRequest,
            IHttpClientFactory httpClientFactory,
            CancellationToken cancellationToken = default)
        {
            var client = httpClientFactory.CreateClient();

            var request = createRequest();
            var response = await client
                .SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}
