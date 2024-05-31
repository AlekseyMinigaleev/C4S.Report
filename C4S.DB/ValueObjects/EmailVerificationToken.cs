using System.Security.Cryptography;

namespace C4S.DB.ValueObjects
{
    /// <summary>
    /// Модель, представляющая токен верификации, присылаемый на почту пользователя.
    /// </summary>
    public class EmailVerificationToken
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
        public EmailVerificationToken CreateEmailConfirmationToken()
        {
            var rng = new Random();

            return new EmailVerificationToken
            {
                Token = rng.Next(100000, 999999).ToString("D6"),
                CreationDate = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Создает токен для сброса пароля почты
        /// </summary>
        public EmailVerificationToken CreateResetPasswordToken()
        {
            var tokenBuffer = new byte[32];
            RandomNumberGenerator.Fill(tokenBuffer);

            return new EmailVerificationToken
            {
                Token = Convert.ToBase64String(tokenBuffer),
                CreationDate = DateTime.UtcNow,
            };
        }

        private EmailVerificationToken()
        { }
    }
}