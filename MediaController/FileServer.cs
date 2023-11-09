using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Reflection.Metadata;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using MediaSyncAPI.Utilities;

namespace MediaSyncAPI.MediaController
{
    public class FileServer
    {
        public static async Task<bool>CheckIfFileExistsNotSupposedToExist(HttpContext context, string path) 
        {
            try
            {
                if (File.Exists(path))
                {
                    await ErrorHandlers.HandleFileAlreadyExistsError(context, "already exists");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e) {
                await ErrorHandlers.HandleFileAlreadyExistsError(context, e.Message);
                return false;
            }
        }


        public static async Task<bool>CheckFileExistsSupposedToExist(HttpContext context, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    await ErrorHandlers.HandleNotFoundError(context, "File not found");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                await ErrorHandlers.HandleNotFoundError(context, ex.Message);
                return false;
            }
        }


        public static async Task StreamFile(HttpContext context, string path)
        {
            try {
                path = $"{MediaList.MediaPath}\\{WebUtility.UrlDecode(path)}";

                bool fileExists = await CheckFileExistsSupposedToExist(context, path);
                if (!fileExists)
                {
                    return;
                }
                Stream stream = File.OpenRead(path);
                FileInfo fileInfo = new FileInfo(path);
                long length = fileInfo.Length;

                if (context.Request.Headers.ContainsKey("Range"))
                {
                    var range = context.Request.Headers["Range"].ToString();
                    var match = Regex.Match(range, @"(\d+)-(\d*)");
                    var start = long.Parse(match.Groups[1].Value);
                    var end = string.IsNullOrEmpty(match.Groups[2].Value) ? length - 1 : long.Parse(match.Groups[2].Value);

                    context.Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{length}");
                    context.Response.StatusCode = (int)HttpStatusCode.PartialContent;

                    stream.Seek(start, SeekOrigin.Begin);
                    length = end - start + 1;
                }

                context.Response.ContentType = "audio/mp3";
                context.Response.Headers.Add("Accept-Ranges", "bytes");

                context.Response.Headers.Add("Content-Length", length.ToString());

                await stream.CopyToAsync(context.Response.Body);
            }
            catch (Exception ex)
            {
                await ErrorHandlers.HandleStreamError(context, ex.Message);
            }
           
        }

        public static async Task HandleDownloadRequest(HttpContext context)  {
            var path = await CheckRetrieveFileFormValid(context);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            await FileServer.DownloadFile(context, path);
        }

        public static async Task<string> CheckRetrieveFileFormValid(HttpContext context)
        {
            var path = context.Request.Query["file"];

            if (string.IsNullOrEmpty(path))
            {
                await ErrorHandlers.HandleEmptyPathError(context, "Empty path");
                return "";
            }
            path = $"{MediaList.MediaPath}\\{WebUtility.UrlDecode(path)}";

            bool exists = await CheckFileExistsSupposedToExist(context, path);
            if (!exists)
            {
                return "";
            }
            return path;
        }


        public static async Task DownloadFile(HttpContext context, string path)
        {
            try {
                string fileName = Path.GetFileName(path);
                Stream fileStream = File.OpenRead(path);

                context.Response.ContentType = "audio/mp3";
                var contentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileNameStar = fileName,
                };
                context.Response.Headers.Add("Content-Disposition", contentDisposition.ToString()); context.Response.Headers.Add("Content-Length", fileStream.Length.ToString());

                await fileStream.CopyToAsync(context.Response.Body);
            }
            catch (Exception error) {
                await ErrorHandlers.HandleStreamError(context, error.Message);
            }
            
        }

        public static async Task UploadFile(HttpContext context)
        {


            try
            {
                if (context.Request.HasFormContentType)
                {
                    var form = await context.Request.ReadFormAsync();

                    var path = form["file"];
                    if (string.IsNullOrEmpty(path))
                    {
                        throw new Exception("Missng file field");
                    }

                    path = $"{MediaList.MediaPath}\\{WebUtility.UrlDecode(path)}";
                    bool notExists = await CheckIfFileExistsNotSupposedToExist(context, path);
                    if (!notExists)
                    {
                        return;
                    }



                    var file = form.Files.First();
                    if (file == null || file.Length == 0 || file.ContentType != "audio/mp3")
                    {
                        throw new Exception("Invalid file");
                    }


                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    context.Response.StatusCode = (int)HttpStatusCode.Created;
                }
                else
                {
                    await ErrorHandlers.HandleEmptyUploadRequestForm(context, "form content type is missing.");
                }
            }
            catch (Exception ex)
            {
                await ErrorHandlers.HandleEmptyUploadRequestForm(context, ex.Message);
            }
        }
        public static async Task<string> CheckDownloadedFileStataus(HttpContext context, string directoryPath) {
            if (FileSystem.GetDirectoryItemsCount(directoryPath) != -1)
            {
                string file = FileSystem.GetFirstAudioFile(directoryPath);
                if (string.IsNullOrEmpty(file))
                {
                    await ErrorHandlers.HandleDownloadRequestFileSystemError(context, "File did not get downloaded");
                    return "";
                }
                else
                {
                    return file;
                }
            }
            else
            {
                await ErrorHandlers.HandleDownloadRequestFileSystemError(context, "File did not get downloaded");
                return "";
            }
        }
        public static async Task DownloadFromYoutube(HttpContext context, string url) {
            var directoryResult = FileSystem.CreateUniqueDirectory();
            if (string.IsNullOrEmpty(directoryResult))
            {
                await ErrorHandlers.HandleDownloadRequestFileSystemError(context, "Failed to create drectory");
                return; 
            }
            var directoryPath = $@".\DownloadedFiles\{directoryResult}\";

            _ = ProcessHandling.RunProcess($"yt-dlp -x {url}", directoryPath);
            string result = await CheckDownloadedFileStataus(context, directoryPath);

            if (!string.IsNullOrEmpty(result))
            {
                await DownloadFile(context, result);
            }

        }

        public static bool CleanUpDownloadedFile(string directoryNameFromRoot) {
            try
            {
                Directory.Delete(directoryNameFromRoot, true);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static async Task HandleDownloadFromYoutubeRequest(HttpContext context)
        {
            try { 
                var url = context.Request.Query["url"];
                if (string.IsNullOrEmpty(url))
                {
                    await ErrorHandlers.HandleEmptyDownloadLinkError(context, "Empty url!");
                    return;
                }
                await DownloadFromYoutube(context, url);
            }
            catch(Exception ex)
            {
                await ErrorHandlers.HandleDownloadRequestError(context, ex.Message); 
            }


        }

        public static async Task HandleCleanUpRequest(HttpContext context)
        {
            try
            {
                bool result = FileSystem.DeleteFilesAndSubdirectories("./DownloadedFiles");
                if (result)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    throw new Exception("Failed to delete the files");
                }
            }
            catch(Exception e)
            {
                await ErrorHandlers.HandleCleanUpRequestSystemError(context, e.Message);
            }
        }

    }
}
