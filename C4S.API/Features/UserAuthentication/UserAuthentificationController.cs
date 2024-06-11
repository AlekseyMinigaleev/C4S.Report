using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace C4S.API.Features.UserAuthentication
{
    public class UserAuthentificationController : BaseApiController
    {
        public UserAuthentificationController(IMediator mediator) : base(mediator)
        { }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPasswordAsync()
        {
            throw new NotImplementedException();
        }
    }
}
