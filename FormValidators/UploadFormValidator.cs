using MediaSyncAPI.MediaController;
using MediaSyncAPI.Utilities;
using System.Net;

namespace MediaSyncAPI.FormValidators
{
    public class UploadFormValidator
    {
        public static async Task<(string Path, IFormFile File)> FormValidator(HttpContext context)
        {
            try
            {
                if (context.Request.HasFormContentType)
                {
                    var form = await context.Request.ReadFormAsync();

                    if (!form.TryGetValue("file", out var pathValues))
                    {
                        throw new BadHttpRequestException("Missing file field");
                    }

                    var path = WebUtility.UrlDecode(pathValues.First());

                    path = Path.Combine(MediaList.MediaPath, path);

                    if (!await FileServer.CheckIfFileExistsNotSupposedToExist(context, path))
                    {
                        return(null, null);
                    }

                    var file = form.Files.First();

                    if (file == null || file.Length == 0)
                    {
                        throw new Exception("Invalid file");
                    }

                    return (path, file);
                }
                else
                {
                    throw new Exception("Empty upload form");
                }
            }
            catch(BadHttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                await ErrorHandlers.HandleEmptyUploadRequestForm(context, ex.Message);
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await ErrorHandlers.HandleFileAlreadyExistsError(context, ex.Message);
                return (null, null);
            }

        }
    }
}
