using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using C4S.API.Features.Game.Actions;
using C4S.API.Attributes;

namespace C4S.API.Features.Game
{
    public class GameController : BaseApiController
    {
        public GameController(IMediator mediator)
            : base(mediator)
        { }

        /// <summary>
        /// Устанавливает PageId для игры. ОТСТУТСТВУЕТ ВАЛИДАЦИЯ НА УСТАНОВКУ PageId ИГРЕ, КОТОРОЙ ЭТОТ PageId НЕ ПРИНАДЛЕЖИТ.
        /// </summary>
        [Authorize]
        [HttpPut("set-pageId")]
        public async Task<ActionResult> SetPageIdsAsync(
            [FromBody] SetPageIds.Command request,
            [FromServices] IValidator<SetPageIds.Command> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, request, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await Mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Возвращает список всех игр
        /// </summary>
        [Authorize]
        [HttpGet("get-games")]
        public async Task<ActionResult> GetGamesAsync(
            [FromQuery] GetGames.Query query,
            CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Возвращает информацию о игре по указанному id
        /// </summary>
        [Authorize]
        [ServiceFilter(typeof(ConfidentialFilterAttribute))]
        [HttpGet("get-game/{id}")]
        public async Task<ActionResult> GetGameByIdAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetGameById.Query()
            {
                Id = id
            };

            var result = await Mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}