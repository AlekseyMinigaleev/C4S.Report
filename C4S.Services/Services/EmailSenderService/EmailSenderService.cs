using C4S.Services.Services.EmailSenderService.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace C4S.Services.Services.EmailSenderService
{
    /// <summary>
    /// <inheritdoc cref="IEmailSenderService"/>
    /// </summary>
    public class EmailSenderService : IEmailSenderService
    {
        /// <summary>
        /// <see cref="EmailSendingConfiguration"/>
        /// </summary>
        public EmailSendingConfiguration EmailConfiguration { get; set; }

        public EmailSenderService(EmailSendingConfiguration emailConfiguration)
        {
            EmailConfiguration = emailConfiguration;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task SendEmailAsync(
            string emailTo,
            string subject,
            TextPart message,
            CancellationToken cancellationToken)
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(EmailConfiguration.EmailUsername));
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;
            email.Body = message;

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                EmailConfiguration.EmailHost,
                EmailConfiguration.SmtpPort,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                EmailConfiguration.EmailUsername,
                EmailConfiguration.EmailPassword);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task SendVerificationCode(
            string emailTo,
            string subject,
            int verificationCode,
            CancellationToken cancellation) =>
            await SendEmailAsync(
                emailTo,
                subject,
                new TextPart(TextFormat.Html)
                {
                    Text = $"<h1>{verificationCode}</h1>"
                },
                cancellation);
    }
}