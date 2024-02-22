﻿using C4S.API.Features;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using С4S.API.Features.User.Action;
using С4S.API.Features.User.Actions;

namespace С4S.API.Features.User
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
        [HttpPut("setRsyaAuthorizationToken")]
        public async Task<ActionResult> SetRsyaAuthorizationTokenAsync(
            [FromBody] SetRsyaAuthorizationToken.Command command,
            [FromServices] IValidator<SetRsyaAuthorizationToken.Command> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, command, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest("Указан некорректный токен авторизации");

            await Mediator.Send(command, cancellationToken);

            return Ok("Токен авторизации успешно установлен");
        }

        /// <summary>
        /// Создает новую учетную запись
        /// </summary>
        [AllowAnonymous]
        [HttpPost("createAccount")]
        public async Task<ActionResult> CreateAccount(
            [FromBody] CreateAccount.Query query,
            [FromServices] IValidator<CreateAccount.Query> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, query, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await Mediator.Send(query, cancellationToken);

            return Ok("Аккаунт успешно создан");
        }
    }
}