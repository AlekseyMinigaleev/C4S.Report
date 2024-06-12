namespace C4S.Services.Services.TokenService
{
    /// <summary>
    /// Сервис для генерации токенов, использующихся для смены критичных настроек.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Шифрует<paramref name="userId"/> в токен, содержащий в себе эти данные.
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <returns>Кортеж, содержащий идентификатор пользователя и дату создания токена.</returns>
        public string GenerateToken(Guid userId);

        /// <summary>
        /// Дешифрует токен и возвращает идентификатор пользователя и дату создания токена.
        /// </summary>
        /// <param name="token">Зашифрованный токен.</param>
        /// <returns>Кортеж, содержащий идентификатор пользователя и дату создания токена.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если формат токена недействителен.</exception>
        /// <exception cref="FormatException">Выбрасывается, если идентификатор пользователя или дата не могут быть разобраны.</exception>
        public (Guid UserId, DateTime Date) DecryptToken(string token);
    }
}