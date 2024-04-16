using C4S.Services.Services.CategoriesSyncService;
using C4S.Shared.Logger;
using MediatR;

namespace С4S.API.Features.Category.Actions
{
    public class SyncCategories
    {
        public class Command : IRequest
        { }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ICategoriesSyncService _categoriesSyncService;
            private readonly ConsoleLogger _logger;

            /*TODO: Добавить доступ только определенной роли*/
            public Handler(
                ICategoriesSyncService categoriesSyncService,
                ILogger<SyncCategories> logger)
            {
                _categoriesSyncService = categoriesSyncService;
                _logger = new ConsoleLogger(logger);
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                await _categoriesSyncService.SyncCategoriesAsync(_logger, cancellationToken);
            }
        }
    }
}