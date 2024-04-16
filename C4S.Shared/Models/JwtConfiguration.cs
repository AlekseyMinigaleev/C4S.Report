namespace C4S.Shared.Models
{
    /// <summary>
    /// Конфигурация JWT токена
    /// </summary>
    public class JwtConfiguration
    {
        /// <summary>
        /// Секретный ключ
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// Издатель
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Аудитория для которой предназначен токен
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        ///<inheritdoc cref="TokensExpiries"/>
        /// </summary>
        public TokensExpiries Expiries { get; set; }
    }

    /// <summary>
    /// Представляет сроки действия токена.
    /// </summary>
    public class TokensExpiries
    {
        /// <summary>
        /// Время жизни токена доступа
        /// </summary>
        public int AccessTokenExpiryInMinutes { get; set; }

        /// <summary>
        /// Время жизни токена обновления
        /// </summary>
        public int RefreshTokenExpiryInDays { get; set; }
    }
}