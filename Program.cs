using System;
using System.IO;
using System.Net.Http;
using VideoLibrary;

try
{
    if (args.Length != 2)
    {
        Console.WriteLine("It's necessary pass the Video Url and the Destination Folder Path.");
        return;
    }

    if (Directory.Exists(args[1]) is false)
    {
        Console.WriteLine("Destination folder not exists.");
        return;
    }

    var youTube = YouTube.Default;
    var video = youTube.GetVideo(args[0]);
    Console.WriteLine($"Video found: {video.FullName}");
    string destination = Path.Combine(args[1], video.FullName);

    var client = new HttpClient();
    long? totalByte = 0;

    using Stream output = File.OpenWrite(destination);
    using var request = new HttpRequestMessage(HttpMethod.Head, video.Uri);
    totalByte = (await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        .Content
        .Headers
        .ContentLength;

    using var input = await client.GetStreamAsync(video.Uri);
    byte[] buffer = new byte[16 * 1024];
    int read, totalRead = 0;

    Console.WriteLine("Download started...");
    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
    {
        output.Write(buffer, 0, read);
        totalRead += read;
        Console.WriteLine($"\rDownloading {totalRead}/{totalByte}");
    }

    Console.WriteLine("Download Completed");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}