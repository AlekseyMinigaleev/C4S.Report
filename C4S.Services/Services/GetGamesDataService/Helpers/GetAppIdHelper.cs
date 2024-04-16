using C4S.Services.Services.GetGamesDataService.Models;
using C4S.Shared.Logger;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace C4S.Services.Services.GetGamesDataService.Helpers
{
    /// <summary>
    /// Вспомогательный класс для получения идентификаторов приложений (<see cref="PublicGameData.AppId"/>) игр из веб-страницы разработчика.
    /// </summary>
    public class GetAppIdHelper
    {
        private readonly IWebDriver _driver;
        private string _developerPageUrl;

        private const string DOCUMENT_STATE_CHECK_READY_SCRIPT = "return document.readyState";
        private const string DOCUMENT_READY_STATE = "complete";

        private const string PAGE_HEIGHT_FETCH_SCRIPT =
            "return Math.max(" +
            " document.body.scrollHeight," +
            " document.body.offsetHeight," +
            " document.documentElement.clientHeight," +
            " document.documentElement.scrollHeight," +
            " document.documentElement.offsetHeight);";

        private const string PAGE_SCROLL_SCRIPT = "window.scrollTo(0, document.body.scrollHeight);";

        public GetAppIdHelper(IWebDriver driver)
        {
            _driver = driver;
        }

        /// <summary>
        /// Получает идентификаторы приложений (<see cref="PublicGameData.AppId"/>) игр из указанной веб-страницы разработчика.
        /// </summary>
        /// <param name="developerPageUrl">URL веб-страницы разработчика.</param>
        /// <param name="logger">Экземпляр логгера для записи информационных сообщений.</param>
        /// <returns>Массив целочисленных значений, представляющих <see cref="PublicGameData.AppId"/> игр.</returns>
        public int[] GetAppIdsAsync(
            string developerPageUrl,
            BaseLogger logger)
        {
            _developerPageUrl = developerPageUrl;

            logger.LogInformation("Начат процесс получения AppId игр");

            logger.LogInformation("Получение url всех игр со страницы разработчика");
            var gameURLs = GetGamesURLs();
            logger.LogSuccess($"Успешно получено {gameURLs.Count()} url игр");

            logger.LogInformation("Получения id из url ир");
            var gameIds = new List<int>();
            foreach (var gameURL in gameURLs)
            {
                var id = GetGameIdFromUrl(gameURL);
                gameIds.Add(id);
            }
            logger.LogSuccess($"Успешно получено {gameIds.Count} id");

            logger.LogSuccess("Процесс получения AppId игр успешно завершен");

            return gameIds.ToArray();
        }

        private IEnumerable<string> GetGamesURLs()
        {
            _driver
               .Navigate()
               .GoToUrl(_developerPageUrl);

            WaitForPageLoad(_driver);

            ScrollToBottom(_driver);

            var gameURLs = _driver
                .FindElements(By.CssSelector(".grid-list"))
                .SelectMany(x => x
                    .FindElements(By.CssSelector(".game-url"))
                    .Where(x => x.TagName == "a")
                    .Select(x => x.GetAttribute("href")))
                .Distinct();

            return gameURLs;
        }

        private static void WaitForPageLoad(IWebDriver driver) =>
            new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                .Until(driver => ((IJavaScriptExecutor)driver)
                    .ExecuteScript(DOCUMENT_STATE_CHECK_READY_SCRIPT).Equals(DOCUMENT_READY_STATE));

        private static void ScrollToBottom(IWebDriver driver)
        {
            var jsDriver = (IJavaScriptExecutor)driver;
            long lastHeight = 0;
            while (true)
            {
                jsDriver.ExecuteScript(PAGE_SCROLL_SCRIPT);

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                try
                {
                    wait.Until(driver => (long)jsDriver.ExecuteScript(PAGE_HEIGHT_FETCH_SCRIPT) != lastHeight);
                    lastHeight = (long)jsDriver.ExecuteScript(PAGE_HEIGHT_FETCH_SCRIPT);
                }
                catch (WebDriverTimeoutException)
                {
                    break;
                }
            }
        }

        private static int GetGameIdFromUrl(string url)
        {
            var gameIdString = GetIdAsString(url);

            if (!int.TryParse(gameIdString, out var gameId))
                throw new FormatException($"не удалось преобразовать id - {gameIdString} в int");

            return gameId;
        }

        private static string GetIdAsString(string path)
        {
            var lastIndex = path.LastIndexOf("/");
            var gameId = path[(lastIndex + 1)..];
            return gameId;
        }
    }
}