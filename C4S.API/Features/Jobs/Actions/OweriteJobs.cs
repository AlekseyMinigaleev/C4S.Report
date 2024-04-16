using C4S.Services.Services.BackgroundJobService;
using C4S.Shared.Logger;
using MediatR;

namespace C4S.API.Features.Jobs.Actions
{
    public class OweriteJobs
    {
        public class Command : IRequest
        { }

        public class Handler : IRequestHandler<Command>
        {
            private readonly IHangfireBackgroundJobService _hangfireBackgroundJobService;
            private readonly ILogger<IHangfireBackgroundJobService> _ILogger;
            private readonly BaseLogger _logger;

            public Handler(
                IHangfireBackgroundJobService hangfireBackgroundJobService,
                ILogger<IHangfireBackgroundJobService> logger)
            {
                _hangfireBackgroundJobService = hangfireBackgroundJobService;
                _ILogger = logger;
                _logger = new ConsoleLogger(_ILogger);
            }

            public async Task Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                /*TODO: пока не реализовано, нужно перезаписать учитывая, что джобы привязаны к пользователям.*/
                //await _hangfireBackgroundJobService.OweriteJobsAsyncs(_logger, cancellationToken);
            }
        }
    }
}