namespace C4S.Shared.Models
{
    /// <summary>
    /// Конфигурация времени жизни токенов, отправляющихся по почте пользователю.
    /// </summary>
    public class EmailTokensExpiration
    {
        /// <summary>
        /// Время жизни токена, для подтверждения почты. Значение в минутах
        /// </summary>
        public int EmailVerificationCodeInMinutes { get; set; }

        /// <summary>
        /// Время жизни токена, для сброса пароля. Значение в минутах
        /// </summary>
        public int ResetPasswordTokenInMinutes { get; set; }
    }
}
