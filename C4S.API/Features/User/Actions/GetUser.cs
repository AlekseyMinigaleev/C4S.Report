using AutoMapper;
using C4S.DB;
using C4S.DB.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using C4S.Shared.Extensions;

namespace С4S.API.Features.User.Actions
{
    public class GetUser
    {
        public class Query : IRequest<ViewModel>
        { }

        public class ViewModel
        {
            public string Email { get; set; }

            public string DeveloperPageUrl { get; set; }

            public string RsyaAuthorizationToken { get; set; }

            public string Password { get; set; }
        }

        public class ViweModelProfiler : Profile
        {
            public ViweModelProfiler()
            {
                CreateMap<UserModel, ViewModel>()
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Login));
            }
        }

        public class Handler : IRequestHandler<Query, ViewModel>
        {
            private readonly IPrincipal _principal;
            private readonly IMapper _mapper;
            private readonly ReportDbContext _dbContext;

            public Handler(
                IPrincipal principal,
                IMapper mapper,
                ReportDbContext dbContext)
            {
                _principal = principal;
                _mapper = mapper;
                _dbContext = dbContext;
            }

            public async Task<ViewModel> Handle(
                Query request,
                CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();

                var user = await _dbContext.Users
                    .SingleAsync(x => x.Id == userId, cancellationToken);

                var viewModel = _mapper.Map<UserModel, ViewModel>(user);

                return viewModel;
            }
        }
    }
}