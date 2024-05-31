using System.Security.Cryptography;

namespace C4S.DB.ValueObjects
{
    /// <summary>
    /// Модель, представляющая токен верификации, присылаемый на почту пользователя.
    /// </summary>
    public class EmailToken
    {
        /// <summary>
        /// Токен
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Дата создания токена
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Создает код подтверждения почты
        /// </summary>
        public static EmailToken CreateEmailConfirmationToken()
        {
            var rng = new Random();

            return new EmailToken
            {
                Token = rng.Next(100000, 999999).ToString("D6"),
                CreationDate = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Создает токен для сброса пароля почты
        /// </summary>
        public static EmailToken CreateResetPasswordToken()
        {
            var tokenBuffer = new byte[32];
            RandomNumberGenerator.Fill(tokenBuffer);

            return new EmailToken
            {
                Token = Convert.ToBase64String(tokenBuffer),
                CreationDate = DateTime.UtcNow,
            };
        }

        private EmailToken()
        { }
    }
}