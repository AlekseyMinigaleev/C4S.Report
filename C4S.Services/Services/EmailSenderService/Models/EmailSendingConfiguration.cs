namespace C4S.Services.Services.EmailSenderService.Models
{
    /// <summary>
    /// Конфигурация для отправки электронной почты.
    /// </summary>
    public class EmailSendingConfiguration
    {
        /// <summary>
        /// Хост электронной почты.
        /// </summary>
        public string EmailHost { get; set; }

        /// <summary>
        /// Имя пользователя электронной почты.
        /// </summary>
        public string EmailUsername { get; set; }

        /// <summary>
        /// Пароль электронной почты.
        /// </summary>
        public string EmailPassword { get; set; }

        /// <summary>
        /// Порт SMTP.
        /// </summary>
        public int SmtpPort { get; set; }
    }
}
