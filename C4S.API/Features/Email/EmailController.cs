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

        [Authorize]
        [HttpPost("SendEmailVerificationCode")]
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

        [Authorize]
        [HttpPost("VerifyEmailVerificationCode")]
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
    }
}