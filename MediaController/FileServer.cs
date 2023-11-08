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

namespace MediaSyncAPI.MediaController
{
    public class FileServer
    {
        private static async Task HandleNotFoundError(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"Error: {error}");
        }

        private static async Task HandleFileAlreadyExistsError(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await context.Response.WriteAsync($"No content: {error}");
        }

        private static async Task HandleEmptyUploadRequestForm(HttpContext context, string error = "") {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync($"Invalid request : {error}");
        }

        private static async Task HandleStreamError(HttpContext context, string error = "")
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync($"internal error : {error}");
        }

        public static async Task<bool>CheckIfFileExistsNotSupposedToExist(HttpContext context, string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    await HandleFileAlreadyExistsError(context, "already exists");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e) {
                await HandleFileAlreadyExistsError(context, e.Message);
                return false;
            }
        }


        public static async Task<bool>CheckFileExistsSupposedToExist(HttpContext context, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    await HandleNotFoundError(context, "File not found");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                await HandleNotFoundError(context, ex.Message);
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
                await HandleStreamError(context, ex.Message);
            }
           
        }

        public static async Task DownloadFile(HttpContext context, string path)
        {
            try {
                path = $"{MediaList.MediaPath}\\{WebUtility.UrlDecode(path)}";
                bool exists = await CheckFileExistsSupposedToExist(context, path);
                if (!exists)
                {
                    return;
                }


                string fileName = Path.GetFileName(path);
                Stream fileStream = File.OpenRead(path);

                context.Response.ContentType = "application/octet-stream";
                var contentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileNameStar = fileName,
                };
                context.Response.Headers.Add("Content-Disposition", contentDisposition.ToString()); context.Response.Headers.Add("Content-Length", fileStream.Length.ToString());

                await fileStream.CopyToAsync(context.Response.Body);
            }
            catch (Exception error) {
                await HandleStreamError(context, error.Message);
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
                    await HandleEmptyUploadRequestForm(context, "form content type is missing.");
                }
            }
            catch (Exception ex)
            {
                await HandleEmptyUploadRequestForm(context, ex.Message);
            }
        }


    }
}
