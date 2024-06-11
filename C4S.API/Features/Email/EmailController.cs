using C4S.API.Features.Email.Actions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace C4S.API.Features.Email
{
    public class EmailController : BaseApiController
    {
        public EmailController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Отправляет код подтверждения на электронную почту пользователя.
        /// </summary>
        /// <param name="validator">Валидатор.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        [Authorize]
        [HttpPost("send-email-verification-code")]
        public async Task<ActionResult> SendEmailVerificationCode(
           [FromServices] IValidator<SendEmailVerificationCode.Command> validator,
           CancellationToken cancellationToken)
        {
            var command = new SendEmailVerificationCode.Command();
            await ValidateAndChangeModelStateAsync(validator, command, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await Mediator.Send(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Проверяет код подтверждения, отправленный на электронную почту пользователя.
        /// </summary>
        /// <param name="query">Запрос на проверку кода подтверждения.</param>
        /// <param name="validator">Валидатор.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        [Authorize]
        [HttpPost("verify-email-verification-code")]
        public async Task<ActionResult> VerifyEmailVerificationCode(
            [FromBody] VerifyEmailVerificationCode.Query query,
            [FromServices] IValidator<VerifyEmailVerificationCode.Query> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, query, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Отправляет ссылку для сброса пароля на указанную электронную почту.
        /// </summary>
        /// <param name="query">Запрос на отправку ссылки для сброса пароля.</param>
        /// <param name="validator">Валидатор.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        [HttpPost("send-reset-password-link")]
        public async Task<ActionResult> SendResetPasswordLink(
            [FromBody] SendResetPasswordLink.Query query,
            [FromServices] IValidator<SendResetPasswordLink.Query> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, query, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await Mediator.Send(query, cancellationToken);

            return Ok();
        }
    }
}