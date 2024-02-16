﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using С4S.API.Extensions;
using С4S.API.Models;

namespace С4S.API.Features.GameStatistic.Actions
{
    public class GetGameStatistics
    {
        public class Query : IRequest<ResponseViewModel>
        {
            public Guid GameId { get; set; }

            public Paginate Paginate { get; set; }

            public Sort Sort { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Paginate)
                    .SetValidator(new PaginateValidator());
            }
        }

        public class ResponseViewModel
        {
            public GameStatisticViewModel[] GameStatistics { get; set; }

            public int TotalCount { get; set; }

            public int RemainingCount { get; set; }
        }

        public class GameStatisticViewModel
        {
            public double Evaluation { get; set; }

            public int PlayersCount { get; set; }

            public double? CashIncome { get; set; }

            public DateTime LastSynchroDate { get; set; }

            public int? Rating { get; set; }
        }

        public class GameStatisticViewModelProfiler : Profile
        {
            public GameStatisticViewModelProfiler()
            {
                CreateMap<GameStatisticModel, GameStatisticViewModel>();
            }
        }

        public class Handler : IRequestHandler<Query, ResponseViewModel>
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

            public async Task<ResponseViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                var gameStatisticsQuery = _dbContext.GamesStatistics
                    .Where(x => x.GameId == request.GameId);

                var totalCount = await gameStatisticsQuery.CountAsync(cancellationToken);

                var remainingCount = await gameStatisticsQuery
                    .Skip(request.Paginate.PageNumber * request.Paginate.ItemsPerPage)
                    .CountAsync(cancellationToken);

                var gameStatistics = await gameStatisticsQuery
                    .OrderBy(request.Sort.GetSortExpression())
                    .Paginate(request.Paginate)
                    .ProjectTo<GameStatisticViewModel>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);

                var response = new ResponseViewModel
                {
                    GameStatistics = gameStatistics,
                    TotalCount = totalCount,
                    RemainingCount = remainingCount
                };

                return response;
            }
        }
    }
}