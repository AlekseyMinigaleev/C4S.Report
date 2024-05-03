using System.Security.Claims;
using System.Security.Principal;

namespace C4S.Shared.Extensions
{
    public static class PrincipalExtension
    {
        /// <summary>
        /// Получает идентификатор пользователя из объекта <see cref="IPrincipal"/>.
        /// </summary>
        /// <param name="principal">Объект  <see cref="IPrincipal"/>, представляющий текущего пользователя.</param>
        /// <exception cref="InvalidOperationException"/>
        /// <returns>Id текущего пользователя</returns>
        public static Guid GetUserId(
            this IPrincipal principal)
        {
            var claimsIdentity = principal.Identity as ClaimsIdentity;

            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                return userId;

            throw new InvalidOperationException(
                $"Не удалось получить или преобразовать идентификатор пользователя из клейма." +
                $" Значение клейма идентификатора пользователя: {userIdClaim}");
        }

        /// <summary>
        /// Получает логин пользователя из объекта <see cref="IPrincipal"/>
        /// </summary>
        /// <param name="principal">Объект  <see cref="IPrincipal"/>, представляющий текущего пользователя.</param>
        /// <exception cref="InvalidOperationException"/>>
        /// <returns>логин текущего пользователя</returns>
        public static string GetUserLogin(this IPrincipal principal)
        {
            var claimsIdentity = principal.Identity as ClaimsIdentity;

            var userLoginClaim = claimsIdentity?.FindFirst(ClaimTypes.Name);

            if (userLoginClaim != null && !string.IsNullOrWhiteSpace(userLoginClaim.Value))
                return userLoginClaim.Value;

            throw new InvalidOperationException(
                $"Не удалось получить или преобразовать логин пользователя из клейма." +
                $" Значение клейма логина пользователя: {userLoginClaim}");
        }
    }
}