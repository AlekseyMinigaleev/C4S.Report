﻿using C4S.DB.Models.Hangfire;
using C4S.Services.Extensions;
using C4S.Services.Interfaces;
using FluentValidation;
using MediatR;

namespace C4S.API.Features.Jobs.Actions
{
    public class UpdateJobs
    {
        public class Command : IRequest<List<ResponseViewModel>>
        {
            /*TODO: почему я не могу передать HangfireJobModel*/
            public HangfireJobConfigurationModel[] UpdatedJobs { get; set; }
        }

        /*TODO: контрукторы для ВМ*/
        public class RequestViewModel
        {
            public HangfireJobTypeEnum JobType { get; set; }

            public string? CronExpression { get; set; }

            public bool IsEnable { get; set; }
        }

        public class ResponseViewModel
        {
            public HangfireJobTypeEnum JobType { get; set; }

            public string? Error { get; set; }
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
            private readonly IBackGroundJobService _backgroundJobService;

            public Handler(IBackGroundJobService backGroundJobService)
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
                    var errors = await UpdateRecurringJobAndGetErrorsAsync(
                        updatedJob,
                        cancellationToken);

                    var responseViewModel = new ResponseViewModel
                    {
                        JobType = updatedJob.JobType,
                        Error = errors
                    };

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
                catch (InvalidCronExpression e)
                {
                    errors = $"{e.Message}: {e.CronExpression}";
                }
                catch (Exception e)
                {
                    errors = $"{e.Message}";
                }

                return errors;
            }

            private async Task UpdateRecurringJobAsync(
                RequestViewModel updatedJob,
                CancellationToken cancellationToken = default)
            {
                var hangfireJobConfiguration = new HangfireJobConfigurationModel(
                            updatedJob.JobType,
                            updatedJob.CronExpression,
                            updatedJob.IsEnable);
            }
        }
    }
}