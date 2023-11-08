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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


MediaList.APIEntry();

app.MapGet("/list", () =>
{
    var forecast = MediaList.APIList();
    return forecast;
})
.WithName("list")
.WithOpenApi();

app.MapGet("/stream", (RequestDelegate)(async (HttpContext context) =>
{
    //string path = "Rock\\01 - Cult Of Personality.flac";
    var path = context.Request.Query["file"];
    await FileServer.StreamFile(context, path);
}));


app.MapGet("/download", (RequestDelegate)(async (HttpContext context) =>
{
    var path = context.Request.Query["file"];
    await FileServer.DownloadFile(context, path);   
}));

app.MapPost("/upload", (RequestDelegate)(async (HttpContext context) =>
{
    await FileServer.UploadFile(context);
}));



app.Run();


