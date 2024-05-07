using C4S.DB;
using C4S.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace C4S.API.Attributes
{
    public class ConfidentialFilterAttribute : ResultFilterAttribute
    {
        private readonly ReportDbContext _dbContext;
        private readonly IPrincipal _principal;

        public ConfidentialFilterAttribute(
            ReportDbContext dbContext,
            IPrincipal principal)
        {
            _dbContext = dbContext;    
            _principal = principal;
        }
            
        public override async void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var userId = _principal.GetUserId();
                var isConfidentialMode = (await _dbContext.UserSettings
                    .SingleAsync(x => x.UserId == userId))
                    .IsConfidentialMode;

                if (isConfidentialMode)
                {
                    var confidentialFields = objectResult.Value
                        ?.GetType()
                        .GetProperties()
                        .Where(x => x
                            .GetCustomAttributes(typeof(ConfidentialAttribute), false)
                            .Length != 0)
                        .ToList();

                    if (confidentialFields != null)
                        foreach (var property in confidentialFields)
                            property.SetValue(objectResult.Value, null);
                }

            }

            base.OnResultExecuting(context);
        }
    }
}