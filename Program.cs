using MediaSyncAPI.MediaController;
using MediaSyncAPI.Utilities;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = null;
});
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

app.MapGet("/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)
));

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


