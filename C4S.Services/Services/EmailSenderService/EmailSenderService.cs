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

        public async Task SendResetPasswordLink(
            string emailTo,
            string subject,
            string resetPasswordLink,
            CancellationToken cancellation)
        {
            await SendEmailAsync(
               emailTo,
               subject,
               new TextPart(TextFormat.Html)
               {
                   Text = @$"
                            <!DOCTYPE html>
                            <html lang=""ru"">
                            <head>
                                <meta charset=""UTF-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                <title>Восстановление пароля</title>
                                <style>
                                    /* Сброс стилей */
                                    body, html {{
                                        margin: 0;
                                        padding: 0;
                                        font-family: Arial, sans-serif;
                                    }}
                                    /* Обертка */
                                    .wrapper {{
                                        width: 100%;
                                        max-width: 600px;
                                        margin: 0 auto;
                                        padding: 20px;
                                    }}
                                    /* Заголовок */
                                    .header {{
                                        background-color: #f0f0f0;
                                        padding: 20px;
                                        text-align: center;
                                    }}
                                    /* Содержимое */
                                    .content {{
                                        padding: 20px;
                                        background-color: #ffffff;
                                    }}
                                    /* Подвал */
                                    .footer {{
                                        background-color: #f0f0f0;
                                        padding: 20px;
                                        text-align: center;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class=""wrapper"">
                                    <div class=""header"">
                                        <h1>Восстановление пароля</h1>
                                    </div>
                                    <div class=""content"">
                                        <p>Здравствуйте,</p>
                                        <p>Вы запросили восстановление пароля. Нажмите ссылку ниже, чтобы восстановить пароль:</p>
                                        <p><a href=""{resetPasswordLink}"">Восстановить пароль.</a></p>
                                        <p>Или скопируйте и вставьте ссылку ниже в поисковую строку</p>
                                        <p><a href=""{resetPasswordLink}"">{resetPasswordLink}</a></p>
                                        <p>Если вы не запрашивали восстановление пароля, просто проигнорируйте это письмо.</p>
                                        <p>Спасибо.</p>
                                    </div>
                                    <div class=""footer"">
                                        <p>Это письмо отправлено автоматически. Пожалуйста, не отвечайте на него.</p>
                                    </div>
                                </div>
                            </body>
                            </html>
                            "
               },
               cancellation);
        }
    }
}