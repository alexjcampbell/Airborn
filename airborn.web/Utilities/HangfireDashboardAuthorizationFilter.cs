using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Check if the user is authenticated and has the AirbornAdmin role
        return httpContext.User.Identity.IsAuthenticated &&
               httpContext.User.IsInRole("AirbornAdmin");
    }
}