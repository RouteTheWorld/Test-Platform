#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

public class RootObject
{
    public List<string> urls { get; set; }
}

public static void Run(string myQueueItem, TraceWriter log)
{
    // TODO: funcionise
    log.Info($"C# Queue trigger function processed: {myQueueItem}");

    // Get a list of urls, nastiness due to utf8 faffing
    HttpClient client = new HttpClient();
    var response = client.GetAsync($"https://byzantine-hornet-5022.twil.io/Upload?roomID={myQueueItem}").Result;

    var buffer = response.Content.ReadAsByteArrayAsync().Result;
    var byteArray = buffer.ToArray();
    var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

    RootObject list = JsonConvert.DeserializeObject<RootObject>(responseString);

    //Setup a clean folder
    var videoPath = "D:\\local\\Temp\\Video";

    System.IO.Directory.CreateDirectory(videoPath);
    System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(videoPath);

    foreach (FileInfo fileInfo in directoryInfo.GetFiles())
    {
        fileInfo.Delete(); 
    }


    // Dowload the files
    WebClient webClient = new WebClient();

    var fileindex = 1;

    foreach(string url in list.urls)
    {
        log.Info(url);
        var filename = "";
        if(url.Contains(".mkv"))
        {
            filename = "mkv" + fileindex + ".mkv";
        }
        else
        {
            filename = "mka" + fileindex + ".mka";
        }
        log.Info(filename);

        var filePath = Path.Combine("D:\\local\\Temp\\Video", filename);

        log.Info(filePath);

        webClient.DownloadFile(url, filePath);
    }

    // D:\\local\\Temp\\Video now has all the files    

    // TODO: hook up python script

    // System.Diagnostics.Process ffmpegProcess = new System.Diagnostics.Process();
    // ffmpegProcess.StartInfo.FileName = @"D:\home\site\wwwroot\QueueTriggerCSharp1\ffmpeg.exe";
    // ffmpegProcess.StartInfo.Arguments = "";
    // ffmpegProcess.StartInfo.UseShellExecute = false;
    // ffmpegProcess.StartInfo.RedirectStandardOutput = true;
    // ffmpegProcess.StartInfo.RedirectStandardError = true;
    // ffmpegProcess.Start();
    // string output = ffmpegProcess.StandardOutput.ReadToEnd();
    // string error = ffmpegProcess.StandardError.ReadToEnd();
    // log.Info(output);
    // log.Info(error);
}
