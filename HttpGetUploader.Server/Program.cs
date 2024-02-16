using System.Collections.Concurrent;

var dataStore = new ConcurrentDictionary<string, string[]>();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/upload", (HttpContext context) =>
{
    var uploadKey = context.Request.Headers["X-Upload-Key"].FirstOrDefault();
    var uploadContent = context.Request.Headers["X-Upload-Content"].FirstOrDefault();
    var uploadPartIndex = context.Request.Headers["X-Upload-Part-Index"].FirstOrDefault();
    var uploadPartsCount = context.Request.Headers["X-Upload-Parts-Count"].FirstOrDefault();


    var appendResult = Append(uploadKey, uploadContent, uploadPartIndex, uploadPartsCount);
    if (!appendResult.Succeed)
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

    return appendResult;
});

app.Run();


AppendResult Append(string? uploadKey, string? content, string? uploadPartIndex, string? uploadPartsCount)
{
    if (string.IsNullOrWhiteSpace(uploadKey)
            || string.IsNullOrWhiteSpace(content)
            || !int.TryParse(uploadPartIndex, out var partIndex)
            || !int.TryParse(uploadPartsCount, out var partsCount)
            || partIndex >= partsCount
            || partIndex < 0)
        return new AppendResult() { Succeed = false };

    if (!dataStore.TryGetValue(uploadKey, out var parts))
        parts = new string[partsCount];
    
    if (partIndex >= parts.Length)
        return new AppendResult() { Succeed = false };

    parts[partIndex] = content;

    dataStore.AddOrUpdate(uploadKey, parts, (a, b) => parts);
    
    var result = new AppendResult() { Succeed = true };

    if (!parts.Any(part => string.IsNullOrEmpty(part)))
    {
        result.CompletelyUploaded = true;
        /*
        var base64FileContent = string.Join("", parts);
        var fileContent = Convert.FromBase64String(base64FileContent);
        */
    }

    return result;
}

class AppendResult
{
    public bool Succeed { get; set; }
    public bool CompletelyUploaded { get; set; }
}