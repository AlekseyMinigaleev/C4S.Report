using C4S.Db.Exceptions;
using C4S.DB;
using C4S.DB.Models.Hangfire;
using C4S.Services.Services.BackgroundJobService;
using C4S.Shared.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace C4S.API.Features.Jobs.Actions
{
    public class UpdateJobs
    {
        public class Command : IRequest<List<ResponseViewModel>>
        {
            /// <summary>
            /// <see cref="HangfireJobConfigurationModel"/>[] с обновленными полями
            /// </summary>
            public UpdateHangfireJobConfigurationDTO[] UpdatedJobs { get; set; }
        }

        public class ResponseViewModel
        {
            /// <summary>
            /// Id джобы
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Текст возможной ошибки при обновлении HangfireConfigurationModel
            /// </summary>
            public string? Error { get; set; }

            public ResponseViewModel(
                Guid id,
                string? error = default)
            {
                Id = id;
                Error = error;
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(
                IPrincipal principal,
                ReportDbContext dbContext)
            {
                RuleFor(x => x)
                    .Must(x => x.UpdatedJobs.GroupBy(x => x.Id).All(group => group.Count() == 1))
                    .WithErrorCode("Error")
                    .WithMessage("Duplicate JobType detected")
                    .Must((x) =>
                    {
                        var userJobs = dbContext.HangfireConfigurations
                            .Where(x => x.UserId == principal.GetUserId())
                            .Select(x => x.Id);

                        if (!x.UpdatedJobs.GroupBy(x => x.Id).All(group => group.Count() == 1))
                            return false;

                        foreach (var job in x.UpdatedJobs)
                            if (!userJobs.Contains(job.Id))
                                return false;

                        return true;
                    })
                    .WithErrorCode("Error")
                    .WithMessage("Invalid jobs in the 'UpdatedJobs' list. Make sure that only the jobs belonging to the current user are listed.");
            }
        }

        public class Handler : IRequestHandler<Command, List<ResponseViewModel>>
        {
            private readonly IHangfireBackgroundJobService _backgroundJobService;

            public Handler(
                IHangfireBackgroundJobService backGroundJobService)
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
                        updatedJob.Id,
                        error);

                    responseViewModelList.Add(responseViewModel);
                }

                return responseViewModelList;
            }

            private async Task<string?> UpdateRecurringJobAndGetErrorsAsync(
                UpdateHangfireJobConfigurationDTO updatedJob,
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