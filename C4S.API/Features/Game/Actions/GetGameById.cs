using AutoMapper;
using C4S.DB;
using C4S.DB.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace С4S.API.Features.Game.Actions
{
    public class GetGameById
    {
        public class Query : IRequest<ResponseViewModel>
        {
            public Guid Id { get; set; }
        }

        public class ResponseViewModel
        {
            public string Name { get; set; }

            public string URL { get; set; }

            public string PreviewURL { get; set; }

            public int? PageId { get; set; }

            public string[] Categories { get; set; }
        }

        public class ResponseViewModelProfiler : Profile
        {
            public ResponseViewModelProfiler()
            {
                CreateMap<GameModel, ResponseViewModel>()
                    .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(x => x.Title)));
            }
        }

        public class Handler : IRequestHandler<Query, ResponseViewModel>
        {
            private readonly ReportDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(
                IMapper mapper,
                ReportDbContext dbContext)  
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<ResponseViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                var game = await _dbContext.Games
                    .Include(x => x.CategoryGameModels)
                        .ThenInclude(x => x.Category)
                    .Include(x => x.User)
                    .Where(x => x.Id == request.Id)
                    .SingleAsync(cancellationToken);

                var responseViewModel = _mapper.Map<ResponseViewModel>(game);

                return responseViewModel;
            }
        }
    }
}   