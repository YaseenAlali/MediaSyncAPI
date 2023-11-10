using MediaSyncAPI.MediaController;
using MediaSyncAPI.Utilities;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
string url = Networking.GenerateUrl();
if (!string.IsNullOrEmpty(url))
{ 
    builder.WebHost.UseUrls(url);
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
bool launchSuccesssful = FileSystem.CreateDownloadedFilesDirectory();
if (!launchSuccesssful)
    return;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


MediaList.APIEntry();

app.MapGet("/list", () =>
{
    var forecast = MediaList.HandleListRequest();
    return forecast;
})
.WithName("list")
.WithOpenApi();

app.MapGet("/stream", (RequestDelegate)(async (HttpContext context) =>
{
    await FileServer.HandleStreamFileRequest(context);
}));


app.MapGet("/download", (RequestDelegate)(async (HttpContext context) =>
{
    await FileServer.HandleDownloadRequest(context);
}));


app.MapPost("/upload", (RequestDelegate)(async (HttpContext context) =>
{
    await FileServer.HandleUploadRequest(context);
}));

app.MapGet("/fetch", (RequestDelegate)(async (HttpContext context) =>
{
    await FileServer.HandleDownloadFromYoutubeRequest(context);
}));

app.MapGet("/cleanup", async (HttpContext context) =>
{
    await FileServer.HandleCleanUpRequest(context);
});



app.Run();


