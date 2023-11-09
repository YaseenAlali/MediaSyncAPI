using System.Net;

namespace MediaSyncAPI.Utilities
{
    public class ErrorHandlers
    {
        public static async Task HandleNotFoundError(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"Error: {error}");
        }

        public static async Task HandleFileAlreadyExistsError(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await context.Response.WriteAsync($"No content: {error}");
        }

        public static async Task HandleEmptyUploadRequestForm(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync($"Invalid request : {error}");
        }

        public static async Task HandleStreamError(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync($"internal error : {error}");
        }

        public static async Task HandleDownloadRequestError(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync($"Internal error : {error}");
        }

        public static async Task HandleEmptyDownloadLinkError(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync($"Bad requset : {error}");
        }

        public static async Task HandleEmptyPathError(HttpContext context, string error = "") {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest; ;
            await context.Response.WriteAsync($"Bad request :  {error}");
        }

        public static async Task HandleDownloadRequestFileSystemError(HttpContext context, string error = "") {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync($"File system error : {error}");
        }

        public static async Task HandleCleanUpRequestSystemError(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync($"File system error : {error}");
        }
    }
}
