using C4S.DB.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace С4S.API.Extensions
{
    /// <summary>
    /// Статический класс с методами-расширениями для интерфейса IPrincipal.
    /// </summary>
    public static class PrincipalExtension
    {
        /// <summary>
        /// Получает токен авторизации для РСЯ пользователя из объекта IPrincipal.
        /// </summary>
        /// <param name="principal">Объект IPrincipal, представляющий текущего пользователя.</param>
        public static string? GetUserRsyaAuthorizationToken(
            this IPrincipal principal)
        {
            var claimsIdentity = principal.Identity as ClaimsIdentity;

            var rsyaAuthorizationToken = claimsIdentity
                ?.FindFirst(nameof(UserModel.RsyaAuthorizationToken))
                ?.Value;

            return rsyaAuthorizationToken;
        }
    }
}
