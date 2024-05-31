using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using C4S.API.Features.User.Action;
using C4S.API.Features.User.Actions;

namespace C4S.API.Features.User
{
    public class UserController : BaseApiController
    {
        public UserController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Возвращает данные о пользователе, отображаемые в профиле.
        /// </summary>
        [Authorize]
        [HttpGet("getUser")]
        public async Task<ActionResult> GetUserAsync(CancellationToken cancellationToken)
        {
            var query = new GetUser.Query();

            var result = await Mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Устанавливает токен авторизации.
        /// https://yandex.ru/dev/partner-statistics/doc/ru/concepts/access
        /// </summary>
        [Authorize]
        [HttpPut("set-rsya-authorization-token")]
        public async Task<ActionResult> SetRsyaAuthorizationTokenAsync(
            [FromBody] SetRsyaAuthorizationToken.Command command,
            [FromServices] IValidator<SetRsyaAuthorizationToken.Command> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, command, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await Mediator.Send(command, cancellationToken);

            return Ok("Токен авторизации успешно установлен");
        }
    }
}