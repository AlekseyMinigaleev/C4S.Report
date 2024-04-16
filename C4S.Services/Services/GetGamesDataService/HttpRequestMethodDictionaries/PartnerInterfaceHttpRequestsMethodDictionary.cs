using C4S.Shared.Models;

namespace C4S.Services.Services.GetGamesDataService.HttpRequestMethodDictionaries
{
    /// <summary>
    /// Словарь запросов <see cref="HttpRequestMessage"/>, необходимых для получения данных по игре с РСЯ.
    /// </summary>
    public static class PartnerInterfaceHttpRequestsMethodDictionary
    {
        /// <summary>
        /// Создает <see cref="HttpRequestMessage"/> для получения отчета по статистике с API РСЯ.
        /// </summary>
        /// <param name="authorizationToken">Токен авторизации.</param>
        /// <param name="pageId">Идентификатор страницы.</param>
        /// <param name="period">Период, за который запрашивается статистика.</param>
        /// <returns>
        /// Объект <see cref="HttpRequestMessage"/> для запроса статистики.
        /// </returns>
        public static HttpRequestMessage GetAppReport(
            string authorizationToken,
            long pageId,
            DateTimeRange period)
        {
            var dateTimeFormat = "yyyy-MM-dd";
            var startDate = period.StartDate.ToString(dateTimeFormat);
            var finishDate = period.FinishDate.ToString(dateTimeFormat);

            /*TODO:Вынести в appsetings*/
            var url = $"https://partner2.yandex.ru/api/" +
                $"statistics2/get.json" +
                $"?pretty=1&" +
                $"currency=RUB&" +
                $"lang=ru&" +
                $"filter=[\"page_id\",\"IN\",\"{pageId}\"]&" +
                $"field=partner_wo_nds&" +
                $"period={startDate}&" +
                $"period={finishDate}";

            var result = new HttpRequestMessage(HttpMethod.Get, url);
            result.Headers.Add("Authorization", authorizationToken);

            return result;
        }
    }
}
