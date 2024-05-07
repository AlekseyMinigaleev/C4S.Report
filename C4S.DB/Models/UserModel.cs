using C4S.DB.Models.Hangfire;
using System.Linq.Expressions;

namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица C4S пользователя
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Ссылка на страницу разработчика
        /// </summary>
        public string DeveloperPageUrl { get; private set; }

        /// <summary>
        /// Токен авторизации
        /// </summary>
        /// <remarks>
        /// РСЯ
        /// </remarks>
        public string? RsyaAuthorizationToken { get; private set; }

        /// <summary>
        /// Токен обновления
        /// </summary>
        public string? RefreshToken { get; private set; }

        /// <summary>
        /// Настройки пользователя
        /// </summary>
        public UserSettingModel Settings { get; set; }
        public Guid SettingId => Settings.Id;

        /// <summary>
        /// Список игр
        /// </summary>
        public ISet<GameModel> Games { get; set; }

        /// <summary>
        /// Список конфигураций джоб
        /// </summary>
        public ISet<HangfireJobConfigurationModel> HangfireJobConfigurationModels { get; private set; }

        public UserModel(
            string login,
            string password,
            string developerPageUrl,
            ISet<GameModel> games,
            string? rsyaAuthorizationToken = default,
            string? refreshToken = null)
        {
            Id = Guid.NewGuid();
            DeveloperPageUrl = NormalizeDeveloperPageUrl(developerPageUrl);
            RsyaAuthorizationToken = rsyaAuthorizationToken;
            Games = games;
            Login = login;
            Password = password;
            RefreshToken = refreshToken;
        }

        private UserModel()
        { }

        /// <summary>
        /// Устанавливает токен авторизации
        /// </summary>
        public void SetAuthorizationToken(string rsyaAuthorizationToken)
        {
            RsyaAuthorizationToken = rsyaAuthorizationToken;
        }

        /// <summary>
        /// Устанавливает токен обновления
        /// </summary>
        public void SetRefreshToken(string? token)
        {
            RefreshToken = token;
        }

        /// <summary>
        /// Удаляет токен обновления
        /// </summary>
        public void ClearRefreshToken()
        {
            RefreshToken = null;
        }

        /// <summary>
        /// Возвращает имя разрботчика из <see cref="DeveloperPageUrl"/>
        /// </summary>
        /// <returns></returns>
        public string GetDeveloperName()
        {
            var developerName = DeveloperPageUrl[(DeveloperPageUrl.IndexOf('=') + 1)..];

            return developerName;
        }

        /// <summary>
        /// Нормализует ссылку на страницу разработчика
        /// </summary>
        /// <param name="developerPageUrl">Ссылка на страницу разработчика</param>
        /// <returns>Нормализованная ссылка на страницу разработчика</returns>
        public static string NormalizeDeveloperPageUrl(string developerPageUrl)
        {
            var index = developerPageUrl.IndexOf("#redir-data");

            if (index > -1)
            {
                var result = developerPageUrl[..index];
                return result;
            }

            return developerPageUrl;
        }
    }

    /// <summary>
    /// Словарь <see cref="Expression"/> для <see cref="UserModel"/>
    /// </summary>
    public static class UserExpressions
    { }
}