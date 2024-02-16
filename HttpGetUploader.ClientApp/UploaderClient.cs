namespace HttpGetUploader.ClientApp;

using System.Net.Http.Json;

public class UploaderClient
{
    const int packageSize = 6 * 1024; // 6 KB

    public async Task<bool> Upload(string apiUrl, string filepath)
    {
        var fileBase64 = Convert.ToBase64String(File.ReadAllBytes(filepath));
        var parts = fileBase64.Chunk(packageSize);
        var uploadKey = Guid.NewGuid().ToString();
        var counter = 0;
        bool completelyUploaded = false;
        foreach (var part in parts)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Add custom headers
                httpClient.DefaultRequestHeaders.Add("X-Upload-Key", uploadKey);
                httpClient.DefaultRequestHeaders.Add("X-Upload-Content", new string(part));
                httpClient.DefaultRequestHeaders.Add("X-Upload-Parts-Count", parts.Count().ToString());
                httpClient.DefaultRequestHeaders.Add("X-Upload-Part-Index", counter.ToString());

                // Make the GET request
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Successful response, read content
                    var result = await response.Content.ReadFromJsonAsync<AppendResult>();
                    if (result?.CompletelyUploaded == true)
                        completelyUploaded = true;
                }
                else
                {
                    // Handle error, retry, etc
                    Console.WriteLine("Error: " + response.StatusCode);
                }
            }

            counter++;
        }

        return completelyUploaded;
    }

    class AppendResult
    {
        public bool Succeed { get; set; }
        public bool CompletelyUploaded { get; set; }
    }
}
