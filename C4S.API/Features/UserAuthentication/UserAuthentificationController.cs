using C4S.API.Features.UserAuthentication.Actions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace C4S.API.Features.UserAuthentication
{
    public class UserAuthentificationController : BaseApiController
    {
        public UserAuthentificationController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Обрабатывает запрос на сброс пароля.
        /// </summary>
        /// <param name="command">Команда для сброса пароля.</param>
        /// <param name="validator">Валидатор для команды сброса пароля.</param>
        /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPasswordAsync(
            [FromBody] ResetPassword.Command command,
            [FromServices] IValidator<ResetPassword.Command> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, command, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await Mediator.Send(command, cancellationToken);

            return Ok();
        }
    }
}
