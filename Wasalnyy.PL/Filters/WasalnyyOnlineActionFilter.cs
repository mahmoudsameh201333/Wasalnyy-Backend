using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Wasalnyy.BLL.Exceptions;

namespace Wasalnyy.PL.Filters
{
    public class WasalnyyOnlineActionFilter : IAsyncActionFilter
    {
        private readonly IWasalnyyHubService _wasalnyyHubService;

        public WasalnyyOnlineActionFilter(IWasalnyyHubService wasalnyyHubService)
        {
            _wasalnyyHubService = wasalnyyHubService;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();

            if (!await _wasalnyyHubService.IsOnlineAsync(userId))
                throw new NotConnectedOnHubException("You are not connected on WasalnyyHub");
            await next();
        }
    }
}
