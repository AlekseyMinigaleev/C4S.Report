using C4S.Services.Services.EmailSenderService.Models;
using MimeKit;

namespace C4S.Services.Services.EmailSenderService
{
    /// <summary>
    /// Служба отправки электронной почты.
    /// </summary>
    public interface IEmailSenderService
    {
        /// <summary>
        /// <see cref="EmailSendingConfiguration"/>
        /// </summary>
        public EmailSendingConfiguration EmailConfiguration { get; set; }

        /// <summary>
        /// Асинхронно отправляет электронное письмо.
        /// </summary>
        /// <param name="email">Адрес электронной почты получателя.</param>
        /// <param name="subject">Тема письма.</param>
        /// <param name="message">Сообщение в формате <see cref="TextPart"/>.</param>
        public Task SendEmailAsync(
            string email,
            string subject,
            TextPart message,
            CancellationToken cancellationToken);

        /// <summary>
        /// Асинхронно отправляет код подтверждения на электронную почту.
        /// </summary>
        /// <param name="emailTo">Адрес электронной почты получателя.</param>
        /// <param name="subject">Тема письма.</param>
        /// <param name="verificationCode">Код подтверждения.</param>
        public Task SendVerificationCode(
          string emailTo,
          string subject,
          int verificationCode,
          CancellationToken cancellation);
    }
}