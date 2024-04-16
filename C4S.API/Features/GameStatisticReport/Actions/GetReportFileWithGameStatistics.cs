using C4S.Services.Services.ExcelWorksheetService;
using C4S.Services.Services.ExcelWorksheetService.Extensions;
using C4S.Shared.Extensions;
using C4S.Shared.Models;
using FluentValidation;
using MediatR;
using System.Security.Principal;
using С4S.API.Features.GameStatisticReport.ViewModels;

namespace С4S.API.Features.GameStatisticReport.Actions
{
    public class GetReportFileWithGameStatistics
    {
        public class Query : IRequest<FileViewModel>
        {
            public DateTimeRange DateRange { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.DateRange)
                    .SetValidator(new DateTimeRangeValidator());

                RuleFor(x => x.DateRange.FinishDate)
                    .LessThan(x => x.DateRange.StartDate.AddYears(1))
                    .WithMessage(x => $"Нельзя выбрать период больше 1 года");
            }
        }

        public class Handler : IRequestHandler<Query, FileViewModel>
        {
            private readonly DetailedReportService _detailedReportFileService;
            private readonly IPrincipal _principal;

            private const string FileName = "S4C.Report.xlsx";
            private const string DetailedStatisticWorksheetName = "Detailed statistics";
            private const string MimeTypeForXLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            public Handler(
               IEnumerable<IExcelWorksheetService> excelFileServices,
               IPrincipal principal)
            {
                _detailedReportFileService = excelFileServices
                    .Resolve<DetailedReportService>();
                _principal = principal;
            }

            public async Task<FileViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _principal.GetUserId();

                using var excelPackage = _detailedReportFileService
                    .AddWithNewPackage(DetailedStatisticWorksheetName, request.DateRange, userId);

                var fileBytes = await excelPackage
                    .GetAsByteArrayAsync(cancellationToken);

                var file = new FileViewModel(fileBytes, FileName, MimeTypeForXLSX);

                return file;
            }
        }
    }
}