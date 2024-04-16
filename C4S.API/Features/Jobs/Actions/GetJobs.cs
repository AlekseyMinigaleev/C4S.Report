using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Models.Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace C4S.API.Features.Jobs.Actions
{
    public class GetJobs
    {
        public class Query : IRequest<ResponseViewModel[]>
        { }

        public class ResponseViewModel
        {
            /// <summary>
            /// Название джобы
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Тип джобы
            /// </summary>
            public HangfireJobType JobType { get; set; }

            /// <summary>
            /// cron выражение
            /// </summary>
            public string? CronExpression { get; set; }

            /// <summary>
            /// статус джобы
            /// </summary>
            public bool IsEnable { get; set; }

            public ResponseViewModel(
                string name,
                HangfireJobType jobType,
                bool isEnable,
                string? cronExpression = default)
            {
                Name = name;
                JobType = jobType;
                CronExpression = cronExpression;
                IsEnable = isEnable;
            }

            private ResponseViewModel()
            { }
        }

        public class ResponseViewModelProfiler : Profile
        {
            public ResponseViewModelProfiler()
            {
                CreateMap<HangfireJobConfigurationModel, ResponseViewModel>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.JobType.GetDisplayName()));
            }
        }

        public class Handler : IRequestHandler<Query, ResponseViewModel[]>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(
                ReportDbContext dbContext,
                IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<ResponseViewModel[]> Handle(
                Query request,
                CancellationToken cancellationToken = default)
            {
                var jobs = await _dbContext.HangfireConfigurations
                    .ProjectTo<ResponseViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                return jobs;
            }
        }
    }
}