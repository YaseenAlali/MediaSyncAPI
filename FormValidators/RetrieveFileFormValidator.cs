using MediaSyncAPI.MediaController;
using MediaSyncAPI.Utilities;
using System.Net;

namespace MediaSyncAPI.FormValidators
{
    public class RetrieveFileFormValidator
    {
        public static async Task<string> FormValidator(HttpContext context)
        {
            var path = context.Request.Query["file"];

            if (string.IsNullOrEmpty(path))
            {
                await ErrorHandlers.HandleEmptyPathError(context, "Empty path");
                return "";
            }
            path = $"{MediaList.MediaPath}\\{WebUtility.UrlDecode(path)}";

            bool exists = await FileServer.CheckFileExistsSupposedToExist(context, path);
            if (!exists)
            {
                return "";
            }
            return path;
        }
    }
}
