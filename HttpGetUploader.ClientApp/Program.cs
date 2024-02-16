// See https://aka.ms/new-console-template for more information
using HttpGetUploader.ClientApp;

var serverUrl = "http://localhost:55555/upload";

Console.WriteLine("Enter a file path to upload to the server at " + serverUrl);
var filepath = Console.ReadLine();
while (filepath != "exit")
{
    if (!string.IsNullOrEmpty(filepath))
    {
        var uploaded = await new UploaderClient().Upload(serverUrl, filepath);
        Console.WriteLine(uploaded ? "File uploaded successfully" : "Upload Failed");
    }

    Console.WriteLine("Enter a file path to upload to the server at " + serverUrl);
    filepath = Console.ReadLine();

}