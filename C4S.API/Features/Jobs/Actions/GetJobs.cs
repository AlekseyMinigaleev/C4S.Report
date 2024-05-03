using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Models.Hangfire;
using C4S.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace C4S.API.Features.Jobs.Actions
{
    public class GetJobs
    {
        public class Query : IRequest<ResponseViewModel[]>
        { }

        public class ResponseViewModel
        {
            /// <summary>
            /// Id джобы
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Название джобы
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// cron выражение
            /// </summary>
            public string? CronExpression { get; set; }

            /// <summary>
            /// статус джобы
            /// </summary>
            public bool IsEnable { get; set; }

            public ResponseViewModel(
                Guid id,
                string name,
                bool isEnable,
                string? cronExpression = default)
            {
                Id = id;
                Name = name;
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
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.JobType.GetName()));
            }
        }

        public class Handler : IRequestHandler<Query, ResponseViewModel[]>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IMapper _mapper;
            private readonly IPrincipal _principal;

            public Handler(
                ReportDbContext dbContext,
                IMapper mapper,
                IPrincipal principal)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _principal = principal;
            }

            public async Task<ResponseViewModel[]> Handle(
                Query request,
                CancellationToken cancellationToken = default)
            {
                var userId = _principal.GetUserId();

                var jobs = await _dbContext.HangfireConfigurations
                    .Where(x=>x.UserId == userId)
                    .ProjectTo<ResponseViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                return jobs;
            }
        }
    }
}