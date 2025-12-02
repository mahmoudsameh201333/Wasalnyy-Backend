namespace Wasalnyy.PL.Middleware
{
    public class ExptionhandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExptionhandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                int statusCode;
                string errorType;
                
                switch (ex)
                {
                    case ArgumentNullException:
                    case ArgumentException:
                    case InvalidOperationException:
                    case DriverIsOfflineException:
                    case OutOfZoneException:
                    case DriverMismatchException:
                    case AlreadyAvailableException:
                    case NotConnectedOnHubException:
                        statusCode = StatusCodes.Status400BadRequest;
                        errorType = "Bad Request";
                        break;

                    case DuplicateNameException:
                    case AlreadyLoggedInWithAnotherDeviceException:
                    case AlreadyInTripException:
                    case TripAlreadyAssignedToDriver:
                        statusCode = StatusCodes.Status409Conflict;
                        errorType = "Conflict";
                        break;

                    case NotFoundException:
                        statusCode = StatusCodes.Status404NotFound;
                        errorType = "Not Found";
                        break;

                    case UnauthorizedAccessException:
                        statusCode = StatusCodes.Status401Unauthorized;
                        errorType = "Unauthorized";
                        break;

                    default:
                        statusCode = StatusCodes.Status500InternalServerError;
                        errorType = "Server Error";
                        break;
                }

                var errorResponse = new
                {
                    StatusCode = statusCode,
                    Error = errorType,
                    Message = ex.Message,
                    Timestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(new ObjectResult(errorResponse)
                {
                    StatusCode = statusCode
                });

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(json);
            }
        }
    }
}