namespace Wasalnyy.PL.Filters
{
    public class SignalRExceptionFilter : IHubFilter
    {
        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            try
            {
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(invocationContext.Hub, ex, invocationContext.HubMethodName);
                throw new HubException(CreateResponseMessage(ex));
            }
        }

        public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context.Hub, ex, "OnConnectedAsync");
                throw new HubException(CreateResponseMessage(ex));
            }
        }

        public async Task OnDisconnectedAsync(
            HubLifetimeContext context,
            Exception? exception,
            Func<HubLifetimeContext, Exception?, Task> next)
        {
            try
            {
                await next(context, exception);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context.Hub, ex, "OnDisconnectedAsync");
                throw new HubException(CreateResponseMessage(ex));
            }
        }

        private async Task HandleExceptionAsync(Hub hub, Exception ex, string methodName)
        {
            var errorResponse = new
            {
                Error = GetErrorType(ex),
                Message = ex.Message,
                Method = methodName,
                Timestamp = DateTime.UtcNow
            };

            await hub.Clients.Caller.SendAsync("ReceiveError", errorResponse);
        }

        private string CreateResponseMessage(Exception ex)
        {
            switch (ex)
            {
                case UnauthorizedAccessException:
                    return "Access denied. Please login again.";
                case ArgumentException:
                case InvalidOperationException:
                case NotFoundException:
                case DriverIsOfflineException:
                case OutOfZoneException:
                case NotConnectedOnHubException:
                    return ex.Message;
                default:
                    return "An unexpected error occurred. Please try again.";
            }
        }

        private string GetErrorType(Exception ex)
        {
            switch (ex)
            {
                case UnauthorizedAccessException:
                    return "Unauthorized";
                case ArgumentNullException:
                case ArgumentException:
                case InvalidOperationException:
                case DriverIsOfflineException:
                case OutOfZoneException:
                case NotConnectedOnHubException:
                    return "Bad Request";
                case NotFoundException:
                    return "Not Found";
                case DuplicateNameException:
                case AlreadyLoggedInWithAnotherDeviceException:
                    return "Conflict";
                default:
                    return "Internal Server Error";
            }
        }
    }
}
