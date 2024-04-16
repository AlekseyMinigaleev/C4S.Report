using C4S.Db.Exceptions;
using C4S.DB.Models.Hangfire;
using C4S.Services.Services.BackgroundJobService;
using FluentValidation;
using MediatR;

namespace C4S.API.Features.Jobs.Actions
{
    public class UpdateJobs
    {
        public class Command : IRequest<List<ResponseViewModel>>
        {
            /// <summary>
            /// <see cref="HangfireJobConfigurationModel"/>[] с обновленными полями
            /// </summary>
            public HangfireJobConfigurationModel[] UpdatedJobs { get; set; }
        }

        public class ResponseViewModel
        {
            /// <summary>
            /// тип джобы
            /// </summary>
            public HangfireJobType JobType { get; set; }

            /// <summary>
            /// Текст возможной ошибки при обновлении HangfireConfigurationModel
            /// </summary>
            public string? Error { get; set; }

            public ResponseViewModel(
                HangfireJobType jobType,
                string? error = default)
            {
                JobType = jobType;
                Error = error;
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x)
                    .Must(x => x.UpdatedJobs
                        .GroupBy(x => x.JobType)
                        .All(group => group.Count() == 1))
                    .WithMessage("Duplicate JobType detected");
            }
        }

        public class Handler : IRequestHandler<Command, List<ResponseViewModel>>
        {
            private readonly IHangfireBackgroundJobService _backgroundJobService;

            public Handler(IHangfireBackgroundJobService backGroundJobService)
            {
                _backgroundJobService = backGroundJobService;
            }

            public async Task<List<ResponseViewModel>> Handle(
                Command command,
                CancellationToken cancellationToken = default)
            {
                var responseViewModelList = new List<ResponseViewModel>();

                foreach (var updatedJob in command.UpdatedJobs)
                {
                    var error = await UpdateRecurringJobAndGetErrorsAsync(
                            updatedJob,
                            cancellationToken);

                    var responseViewModel = new ResponseViewModel(
                        jobType: updatedJob.JobType,
                        error: error);

                    responseViewModelList.Add(responseViewModel);
                }

                return responseViewModelList;
            }

            private async Task<string?> UpdateRecurringJobAndGetErrorsAsync(
                HangfireJobConfigurationModel updatedJob,
                CancellationToken cancellationToken)
            {
                string errors = null;
                try
                {
                    await _backgroundJobService.UpdateRecurringJobAsync(updatedJob, cancellationToken);
                }
                catch (InvalidCronExpressionException e)
                {
                    errors = $"{e.Message}: {e.CronExpression}";
                }
                catch (Exception e)
                {
                    errors = $"{e.Message}";
                }

                return errors;
            }
        }
    }
}