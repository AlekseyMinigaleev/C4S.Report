using C4S.API.Features;
using C4S.Shared.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using С4S.API.Features.GameStatisticReport.Actions;

namespace С4S.API.Features.GameStatisticReport
{
    public class GameStatisticReportController : BaseApiController
    {
        public GameStatisticReportController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Возвращает готовый к скачиванию файл отчета игровой статистики.
        /// </summary>
        /// <remarks>
        /// dd.MM.yyyy
        /// </remarks>
        [Authorize]
        [HttpGet("DownloadReport")]
        public async Task<ActionResult> GetGameStatisticReportAsync(
            [FromQuery] string startDate,
            [FromQuery] string finishDate,
            [FromServices] IValidator<GetReportFileWithGameStatistics.Query> queryValidator,
            CancellationToken cancellationToken = default)
        {
            /*TODO: исправать после добавления фронта*/
            var dateTimeFormat = "dd.MM.yyyy";
            var dateTimeRange = new DateTimeRange(
                startDate: DateTime.ParseExact(startDate, dateTimeFormat, CultureInfo.InvariantCulture),
                finishDate: DateTime.ParseExact(finishDate, dateTimeFormat, CultureInfo.InvariantCulture));

            var query = new GetReportFileWithGameStatistics.Query { DateRange = dateTimeRange };

            await ValidateAndChangeModelStateAsync(queryValidator, query, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var file = await Mediator.Send(query, cancellationToken);

            return File(file.Bytes, file.MimeType, file.Name);
        }
    }
}