using C4S.API.Features;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using C4S.API.Features.Authentication.Actions;
using C4S.API.Features.Authentication.Models;

namespace C4S.API.Features.Authentication
{
    public class AuthenticationController : BaseApiController
    {
        public AuthenticationController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Осуществляет аутентификацию пользователя
        /// </summary>
        /// <returns>
        /// Jwt-токен пользователя
        /// </returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(
            [FromBody] Login.Query query,
            [FromServices] IValidator<Login.Query> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, query, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Mediator.Send(query, cancellationToken);

            Response.Cookies.Append(
                nameof(AuthorizationTokens.RefreshToken),
                result.AuthorizationTokens.RefreshToken,
                new CookieOptions { HttpOnly = true });

            return Ok(result);
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

        /// <summary>
        /// Выполняет удаление токена обновления.
        /// </summary>
        [Authorize]
        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteRefreshToken(CancellationToken cancellationToken)
        {
            var command = new DeleteRefreshToken.Command();

            await Mediator.Send(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Выполняет обновление токена доступа
        /// </summary>
        [AllowAnonymous]
        [HttpGet("refresh")]
        public async Task<ActionResult> RefreshAccessToken(
            CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[nameof(AuthorizationTokens.RefreshToken)];

            if (refreshToken is null)
                return Unauthorized();

            var command = new RefreshAccessToken.Command
            {
                RefreshToken = refreshToken,
            };

            var accessToken = await Mediator.Send(command, cancellationToken);

            if (accessToken is null)
                return Unauthorized();

            return Ok(new { AccessToken = accessToken });
        }

        /// <summary>
        /// Выполняет выход из аккаунта
        /// </summary>
        [Authorize]
        [HttpGet("logout")]
        public async Task<ActionResult> LogoutAsync(CancellationToken cancellationToken)
        {
            var command = new Logout.Command();

            await Mediator.Send(command, cancellationToken);

            Response.Cookies.Delete(
               nameof(AuthorizationTokens.RefreshToken),
               new CookieOptions { HttpOnly = true });

            return Ok();
        }
    }
}