# HttpGetUploader

The two simple projects are just for proof of concept, if you can upload a file using HttpGet method.

## WTF? What is the idea?
The idea is very simple: the size of every HttpGet request can be depending of the web server >= 8 KB. So we can send the file using custom request headers (headers starting with `X-`). If the file is larger, we split it into 8KB parts (or 6KB for safety). There is a specific endpoint on server which understands these headers, combines file parts, sent by multiple calls, and at the end we have the complete file on server.

## WTF? WHY?
It's possible! It's not the best idea ever. But makes you know more about http requests. and you will find your use-case.

## WTF? What is done in this sample project?
### the client
1. generates a unique key for the file (that server can identify, about which file is our request).
1. converts the file content to base64 string.
1. splits the base64 string into 6KB chunks, and will create one request per chunk. So if the file which you want to upload is 15KB, the client will create 3 requests to the server, sending one chunk in each request.
1. because the server should know the total chunks count, and which part the client have sent in the request, the client supplies some meta data as custom headers.

#### the headers sent by every request
- X-Upload-Key: the unique key of the whole file. So the server can combine the parts of the file.
- X-Upload-Content: the data part. It's the partial content of the original file.
- X-Upload-Part-Index: the index of the current chunk.
- X-Upload-Parts-Count: the total count of chunks.

### the server
1. reads the custom headers and validates them
1. uses an in memory concurrentDictionary to store the file parts, with the upload-key.
1. puts the uploaded content into the correct index of the final file chunks
1. if all parts are stored, returns CompletelyUploaded=true property in the result object. Because it's just a poc, I have not saved the file, or have not checked if the files are valid, healthy, non-dangerous, etc.

## Why not to send the file at once?
It's possible. you can configure the web server (the server app), to accept larger http get requests.
