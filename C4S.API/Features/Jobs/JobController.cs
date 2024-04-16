using C4S.API.Features.Jobs.Actions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using C4S.API.Features.Jobs.Actions;

namespace C4S.API.Features.Jobs
{
    public class JobController : BaseApiController
    {
        public JobController(
            IMediator mediator)
            : base(mediator)
        { }

        /// <summary>
        ///  Выдает конфигурации всех запланированных джоб.
        /// </summary>
        [HttpGet("GetAllJobs")]
        public async Task<ActionResult<GetJobs.ResponseViewModel[]>> GetJobsAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await Mediator.Send(new GetJobs.Query(), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Выполняет обновление конфигураций всех запланированных джоб.
        /// </summary>
        [HttpPost("UpdateAllJobs")]
        public async Task<ActionResult<UpdateJobs.ResponseViewModel>> UpdateJobsAsync(
            UpdateJobs.Command request,
            IValidator<UpdateJobs.Command> validator,
            CancellationToken cancellationToken)
        {
            await ValidateAndChangeModelStateAsync(validator, request, cancellationToken);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Запускает процесс перезаписи всех джоб в hangfire базе данных
        /// </summary>
        [HttpGet("OweriteAllJobs")]
        public async Task<IActionResult> OweriteJobsAsyc(
            CancellationToken cancellationToken)
        {
            var command = new OweriteJobs.Command();

            await Mediator.Send(command, cancellationToken);

            return Ok();
        }
    }
}