using C4S.API.Features;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using C4S.API.Features.GameStatistic.Actions;

namespace C4S.API.Features.GameStatistic
{
    public class GameStatisticController : BaseApiController
    {
        public GameStatisticController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Возвращает список записей статистики по игре
        /// </summary>
        [Authorize]
        [HttpGet("get-statistic-by-game")]
        public async Task<ActionResult> GetGameStatisticsAsync(
            [FromQuery] GetGameStatistics.Query query,
            [FromServices] IValidator<GetGameStatistics.Query> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, query, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
