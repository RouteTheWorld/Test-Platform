#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

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

        var filePath = Path.Combine(videoPath, filename);

        log.Info(filePath);

        webClient.DownloadFile(url, filePath);
    }

    // D:\\local\\Temp\\Video now has all the files


    // Get all the start times out using ffprobe
    DirectoryInfo d = new DirectoryInfo(videoPath);

    Dictionary<string, float> fileStartTimes = new Dictionary<string, float>();

    float minStartTime = float.MaxValue;

    foreach (var file in d.GetFiles("*.*"))
    {
        var fileName = file.Name;
        log.Info(fileName);
        var filePath = Path.Combine(videoPath, fileName);

        System.Diagnostics.Process ffprobeProcess = new System.Diagnostics.Process();
        ffprobeProcess.StartInfo.FileName = @"D:\home\site\wwwroot\QueueTriggerCSharp1\ffprobe2.exe";
        var arguments =  "-show_entries format=start_time -loglevel quiet " + filePath;
        log.Info(arguments);
        ffprobeProcess.StartInfo.Arguments = arguments;
        ffprobeProcess.StartInfo.UseShellExecute = false;
        ffprobeProcess.StartInfo.RedirectStandardOutput = true;
        ffprobeProcess.StartInfo.RedirectStandardError = true;
        ffprobeProcess.Start();
        string output = ffprobeProcess.StandardOutput.ReadToEnd();
        string error = ffprobeProcess.StandardError.ReadToEnd();
        log.Info(output);
        log.Info(error);

        Regex regex = new Regex(@"[0-9]+\.[0-9]+");
        Match match = regex.Match(output);
        log.Info("startTime: " + match.Value);

        float startTime = float.Parse(match.Value);
        minStartTime = Math.Min(startTime, minStartTime);
        fileStartTimes.Add(filePath, startTime);
    }
   
    string ffprobeAruguments = "ffmpeg";

    foreach(KeyValuePair<string, float> fileInfo in fileStartTimes)
    {
        if(fileInfo.Key.Contains(".mkv"))
        {
            ffprobeAruguments = ffprobeAruguments + "-i" + fileInfo.Key + " ";
        }
        else
        {
            ffprobeAruguments = ffprobeAruguments + "-i" + fileInfo.Key + "-acodec libopus ";
        }
    }
    ffprobeAruguments = ffprobeAruguments + "-y -filter-complex \"";
}
