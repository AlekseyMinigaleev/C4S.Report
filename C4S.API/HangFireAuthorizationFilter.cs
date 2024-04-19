using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace C4S.API
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}