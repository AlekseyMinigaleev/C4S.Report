using C4S.DB.Models;
using C4S.Services.Services.GetGamesDataService.Enums;
using C4S.Services.Services.GetGamesDataService.Models;
using C4S.Shared.Extensions;
using Newtonsoft.Json;
using System.Text;

namespace C4S.Services.Services.GetGamesDataService.RequestMethodDictionaries
{
    /// <summary>
    /// Словарь, содержащий все <see cref="HttpRequestMessage"/>, необходимые для получения данных по игре со страницы разработчика
    /// </summary>
    public static class YGApiHttpRequestMethodDictionary
    {
        /// <summary>
        /// Создает <see cref="HttpRequestMessage"/> для получения <see cref="PublicGameData"/>.
        /// </summary>
        /// <param name="requestUrl">url запроса</param>
        /// <param name="appID">id <see cref="GameModel"/>, по которому необходимо получить информацию</param>
        /// <param name="format">формат ответа</param>.
        /// <returns><see cref="HttpRequestMessage"/></returns>.
        public static HttpRequestMessage GetGamesInfo(string requestUrl, int appID, RequestFormat format)
        {
            var requestData = new
            {
                appID,
                format = format.GetName(),
            };

            var jsonPayload = JsonConvert.SerializeObject(requestData);

            var payload = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var result = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = payload
            };

            return result;
        }
    }
}