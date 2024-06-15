using AutoMapper;
using AutoMapper.QueryableExtensions;
using C4S.DB;
using C4S.DB.Extensions;
using C4S.DB.Models;
using C4S.DB.ValueObjects;
using C4S.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Security.Principal;
using System.Text.Json.Serialization;
using C4S.API.Extensions;
using C4S.API.Models;

namespace C4S.API.Features.Game.Actions
{
    public class GetGames
    {
        public class Query : IRequest<ResponseViewModel>
        {
            public Paginate Paginate { get; set; }

            public Sort Sort { get; set; }
        }

        public class ResponseViewModel
        {
            public GameViewModel[] Games { get; set; }

            public TotalViewModel Total { get; set; }
        }

        public class TotalViewModel
        {
            public double? CashIncome { get; set; }
            public int Count { get; set; }
        }

        public class CashIncomeViewModel : IComparable<CashIncomeViewModel>
        {
            public ValueWithProgress<double>? ValueWithProgress { get; set; }
            public double? Percentage { get; set; }

            public int CompareTo(CashIncomeViewModel? obj)
            {
                if (obj is null)
                    return 1;

                return Comparer<ValueWithProgress<double>>.Default
                    .Compare(ValueWithProgress, obj.ValueWithProgress);
            }
        }

        public class GameViewModel
        {
            public Guid Id { get; set; }

            public string? Name { get; set; }

            public DateTime? PublicationDate { get; set; }

            public string URL { get; set; }

            public string PreviewURL { get; set; }

            public string[] Categories { get; set; }

            public double Evaluation => LastSynchronizationStatistic.Evaluation;

            public ValueWithProgress<int>? Rating => LastSynchronizationStatistic.Rating;

            public CashIncomeViewModel? CashIncome =>
                LastSynchronizationStatistic.CashIncome is not null
                    ? new() { ValueWithProgress = LastSynchronizationStatistic.CashIncome }
                    : null;

            [JsonIgnore]
            public GameStatisticModel LastSynchronizationStatistic { get; set; }
        }

        public class GameViewModelProfiler : Profile
        {
            public GameViewModelProfiler()
            {
                CreateMap<GameModel, GameViewModel>()
                    .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryGameModels
                        .Select(x => x.Category.Title)))
                    .ForMember(dest => dest.LastSynchronizationStatistic, opt => opt.MapFrom(src => src.GameStatistics.GetLastSynchronizationStatistic()))

                    .ForMember(dest => dest.CashIncome, opt => opt.Ignore())
                    .ForMember(dest => dest.Evaluation, opt => opt.Ignore())
                    .ForMember(dest => dest.Rating, opt => opt.Ignore());
            }
        }

        public class Handler : IRequestHandler<Query, ResponseViewModel>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IPrincipal _principal;
            private readonly IMapper _mapper;

            public Handler(
                ReportDbContext dbContext,
                 IMapper mapper,
                IPrincipal principal)
            {
                _dbContext = dbContext;
                _principal = principal;
                _mapper = mapper;
            }

            public async Task<ResponseViewModel> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();

                var allGames = await _dbContext.Games
                    .Where(x => x.UserId == userId && !x.IsArchived)
                    .Include(x => x.User)
                    .ProjectTo<GameViewModel>(_mapper.ConfigurationProvider)
                    .AsSplitQuery() /*TODO: не совсем понимаю как это работает, но это решает warning*/
                    .ToArrayAsync(cancellationToken);

                var paginatedGames = allGames
                    .AsQueryable()
                    .OrderBy(request.Sort.GetSortExpression())
                    .Paginate(request.Paginate)
                    .ToArray();

                var response = new ResponseViewModel
                {
                    Games = paginatedGames,
                    Total = new()
                    {
                        CashIncome = allGames.Any(x => x.CashIncome?.ValueWithProgress is not null)
                                ? allGames.Sum(x => x.CashIncome?.ValueWithProgress?.ActualValue)
                                : null,
                        Count = allGames.Length
                    }
                };

                EnrichResponse(response);

                return response;
            }

            private static void EnrichResponse(ResponseViewModel response)
            {
                foreach (var game in response.Games)
                    if (game.CashIncome?.ValueWithProgress is not null) /*game.CashIncome?.ValueWithProgress is not null если хоть 1 game имеет значение cashIncome, то total точно not null*/
                        game.CashIncome.Percentage = CalculatePercentage(
                            game.CashIncome.ValueWithProgress.ActualValue,
                            response.Total.CashIncome!.Value);
            }

            private static double CalculatePercentage<T>(T value, T total)
                where T : IConvertible
            {
                var a = Convert.ToDouble(value);
                var b = Convert.ToDouble(total);

                double percentage = (a / b) * 100;

                var roundedPercentage = Math.Round(percentage, 3);

                return roundedPercentage;
            }
        }
    }
}