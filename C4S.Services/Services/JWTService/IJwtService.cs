using C4S.DB.Models;
using C4S.Shared.Models;
using Microsoft.IdentityModel.Tokens;

namespace C4S.Services.Services.JWTService
{
    /// <summary>
    /// Сервис для работы с JWT.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Возвращает дату истечения срока действия токена доступа.
        /// </summary>
        public DateTime AccessTokenExpiry { get; }

        /// <summary>
        /// Возвращает дату истечения срока действия токена обновления.
        /// </summary>
        public DateTime RefreshTokenExpiry { get; }

        /// <summary>
        /// Возвращает объект типа <see cref="SecurityKey"/> на основе установленного ключа безопасности.
        /// </summary>
        public SecurityKey SymmetricSecurityKey { get; }

        /// <summary>
        /// Возвращает конфигурацию JWT.
        /// </summary>
        public JwtConfiguration JwtConfig { get; }

        /// <summary>
        /// Создает JWT-токен на основе предоставленных утверждений (claims) и указанной даты истечения срока действия.
        /// </summary>
        /// <param name="user">Модель пользователя, для которой создается токен.</param>
        /// <param name="expiry">Дата истечения срока действия токена.</param>
        /// <returns>Строка, представляющая собой JWT-токен.</returns>
        public string CreateJwtToken(UserModel user, DateTime expiry);
    }
}