using System.Diagnostics;
using System.Net;
using System.Text;

namespace PowerExtensions
{
    public delegate string PostRequestHandle(string request, string requestUrl);
    public class HttpServer
    {
        public PostRequestHandle PostRequestHandle;
        bool runServer;
        string websitePath;
        HttpListener listener;
        public HttpServer(int port, string websitePath)
        {
            runServer = false;
            this.websitePath = websitePath;
            listener = new();
            listener.Prefixes.Add("http://localhost:" + port + "/");
        }

        public void Start()
        {
            listener.Start();
            runServer = true;
            Task.Run(Run);
        }

        public void Stop()
        {
            runServer = false;
            listener.Stop();
        }

        async void Run()
        {
            string phpZipPath = Path.GetTempFileName();
            string phpExtractPath = Path.GetTempFileName();
            File.Delete(phpExtractPath);
            Directory.CreateDirectory(phpExtractPath);
            File.WriteAllBytes(phpZipPath, Properties.Resources.PHP);
            await Task.Run(() => Compression.ExtractZipFile(phpZipPath, phpExtractPath));
            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                string reqPath = req.Url.AbsolutePath == "/" ? "index" : req.Url.AbsolutePath.Substring(1);
                byte[] pageData = Encoding.Default.GetBytes("<!DOCTYPE html><html lang='en'><head><link href='https://fonts.googleapis.com/css2?family=Nunito+Sans:wght@600;900&display=swap' rel='stylesheet'><script src='https://kit.fontawesome.com/4b9ba14b0f.js' crossorigin='anonymous'></script></head><body><div class='mainbox'><div class='err'>4</div><i class='far fa-question-circle fa-spin'></i><div class='err2'>4</div><div class='msg'>Maybe this page moved? Got deleted? Is hiding out in quarantine? Never existed in the first place?<p>Let's go <a href='/'>home</a> and try from there.</p></div></div></body><style>body{background-color:#95c2de}.mainbox{background-color:#95c2de;margin:auto;height:600px;width:600px;position:relative}.err{color:#ffffff;font-family:'Nunito Sans', sans-serif;font-size:11rem;position:absolute;left:20%;top:8%}.far{position:absolute;font-size:8.5rem;left:42%;top:15%;color:#ffffff}.err2{color:#ffffff;font-family:'Nunito Sans', sans-serif;font-size:11rem;position:absolute;left:68%;top:8%}.msg{text-align:center;font-family:'Nunito Sans', sans-serif;font-size:1.6rem;position:absolute;left:16%;top:45%;width:75%}a{text-decoration:none;color:white}a:hover{text-decoration:underline}</style></html>");
                if (req.HttpMethod == "POST")
                {
                    StreamReader reader = new(req.InputStream, req.ContentEncoding);
                    string postReq = PostRequestHandle.Invoke(reader.ReadToEnd(), reqPath);
                    if (postReq != "") { await resp.OutputStream.WriteAsync(Encoding.Default.GetBytes(postReq), 0, Encoding.Default.GetByteCount(postReq)); }
                    else { await resp.OutputStream.WriteAsync(pageData, 0, pageData.Length); }
                    resp.Close();
                    reader.Close();
                }
                else
                {
                    string file = Path.Combine(websitePath, reqPath);
                    bool usePHPRenderer = true;
                    if (File.Exists(file + ".html") && string.IsNullOrEmpty(Path.GetExtension(file))) { file += ".html"; }
                    else if (File.Exists(file + ".htm") && string.IsNullOrEmpty(Path.GetExtension(file))) { file += ".htm"; }
                    else if (File.Exists(file + ".php") && string.IsNullOrEmpty(Path.GetExtension(file))) { file += ".php"; }
                    else if (string.IsNullOrEmpty(Path.GetExtension(file))) { usePHPRenderer = false; }
                    if (usePHPRenderer)
                    {
                        Process proc = new();
                        proc.StartInfo.FileName = Path.Combine(phpExtractPath, "php.exe");
                        proc.StartInfo.Arguments = "-d \"display_errors=1\" -d \"error_reporting=E_PARSE\" \"" + file + "\"";
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.RedirectStandardError = true;
                        proc.Start();
                        pageData = Encoding.Default.GetBytes(proc.StandardOutput.ReadToEnd());
                        if (string.IsNullOrEmpty(Encoding.Default.GetString(pageData)))
                        {
                            pageData = Encoding.Default.GetBytes("Error" + pageData);
                            proc.StandardError.Close();
                        }
                        else { proc.StandardOutput.Close(); }
                        proc.Close();
                        resp.ContentLength64 = pageData.LongLength;
                        await resp.OutputStream.WriteAsync(pageData, 0, pageData.Length);
                        resp.Close();
                    }
                    else
                    {
                        if (File.Exists(file)) { pageData = File.ReadAllBytes(file); }
                        await resp.OutputStream.WriteAsync(pageData, 0, pageData.Length);
                        resp.Close();
                    }
                }
            }
        }
    }
}
