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
        public string Email { get; private set; }

        /// <summary>
        /// Активный ли аккаунт. Аккаунт считается активным, если была выполнена первая синхронизация данных.
        /// </summary>
        public bool IsActive { get; set; }

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
        /// Список игр
        /// </summary>
        public ISet<GameModel> Games { get; set; }

        /// <summary>
        /// Список конфигураций джоб
        /// </summary>
        public ISet<HangfireJobConfigurationModel> HangfireJobConfigurationModels { get; private set; }

        /// <summary>
        /// авторизационные данные пользователя
        /// </summary>
        public UserAuthenticationModel? AuthenticationModel { get; private set; }

        public UserModel(
            string email,
            string developerPageUrl,
            ISet<GameModel> games,
            UserAuthenticationModel? authenticationModel = default,
            string? rsyaAuthorizationToken = default)
        {
            Id = Guid.NewGuid();
            DeveloperPageUrl = NormalizeDeveloperPageUrl(developerPageUrl);
            RsyaAuthorizationToken = rsyaAuthorizationToken;
            Games = games;
            Email = email;
            AuthenticationModel = authenticationModel;
        }

        private UserModel()
        { }

        /// <summary>
        /// Устанавливает токен авторизации
        /// </summary>
        public void SetRsyaAuthorizationToken(string rsyaAuthorizationToken) =>
            RsyaAuthorizationToken = rsyaAuthorizationToken;

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