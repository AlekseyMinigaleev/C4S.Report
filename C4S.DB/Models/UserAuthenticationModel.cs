using C4S.DB.ValueObjects;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица авторизационных данных пользователя
    /// </summary>
    public class UserAuthenticationModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// FK
        /// </summary>
        public Guid UserId { get; private set; }

        public UserModel User { get; private set; }

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public byte[] PasswordHash { get; private set; }

        /// <summary>
        /// Соль пароля
        /// </summary>
        public byte[] PasswordSalt { get; private set; }

        /// <summary>
        /// Токен обновления
        /// </summary>
        public string? RefreshToken { get; private set; }

        /// <summary>
        /// Код для подтверждения почты 
        /// </summary>
        public EmailVerificationToken? EmailVerificationCode { get; set; }

        public UserAuthenticationModel(
            UserModel user,
            string password,
            string? refreshToken = default)
        {
            Id = Guid.NewGuid();
            User = user;
            UserId = user.Id;
            SetPassword(password);
            RefreshToken = refreshToken;
        }

        private UserAuthenticationModel()
        { }

        /// <summary>
        /// Устанавливает значения для полей <see cref="PasswordHash"/> и <see cref="PasswordSalt"/>
        /// </summary>
        /// <param name="password">Пароль пользователя</param>
        public void SetPassword(string password)
        {
            PasswordSalt = RandomNumberGenerator.GetBytes(16);

            /*TODO: по хорошему надо вынести все настройки argon2 в appsettings*/
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = PasswordSalt,
                DegreeOfParallelism = 8,
                MemorySize = 1024 * 1024,
                Iterations = 4
            };
            PasswordHash = argon2.GetBytes(16);
        }

        /// <summary>
        /// Проверяет введенный пароль на соответствие хэшу пароля пользователя.
        /// </summary>
        /// <param name="password">Пароль, который необходимо проверить.</param>
        /// <returns> <see langword="true"/>, если введенный пароль соответствует хэшу пароля пользователя, иначе <see langword="false"/>.</returns>
        public bool ValidatePassword(string password)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = PasswordSalt,
                DegreeOfParallelism = 8,
                MemorySize = 1024 * 1024,
                Iterations = 4
            };
            var hashToVerify = argon2.GetBytes(16);
            return PasswordHash.SequenceEqual(hashToVerify);
        }

        /// <summary>
        /// Устанавливает токен обновления
        /// </summary>
        public void SetRefreshToken(string? token) =>
            RefreshToken = token;

        /// <summary>
        /// Создает и устанавливает код для подтверждения почты
        /// </summary>
        public void GenerateAndSetEmailVerificationCode() =>
            EmailVerificationCode = EmailVerificationCode.CreateEmailConfirmationToken();
    }
}