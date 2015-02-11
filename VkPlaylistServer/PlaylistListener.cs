using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace VkPlaylistServer
{
    public class PlaylistListener
    {
        private VkPlaylistServer.PlaylistRequester plreq;
        private bool listening = false;
        private HttpListener listener = new HttpListener();

        public PlaylistListener(string uname, string password, int port) {
            this.plreq = new VkPlaylistServer.PlaylistRequester(uname, password);
            this.listener.Prefixes.Add("http://*:" + port.ToString() + "/");
        }

        public void StopListening() {
            if (this.listening) {
                listener.Stop();
            }
            this.listening = false;
        }

        public async void Listen() {
            this.listening = true;
            listener.Start();
            while (listening) {
                try
                {
                    var reqcontext = await listener.GetContextAsync();
                    Task.Factory.StartNew(() => SendPlaylist(reqcontext));
                }
                catch { }
            }
        }

        private void SendPlaylist(HttpListenerContext ReqContext) {
            var url = ReqContext.Request.RawUrl;
            url = System.Web.HttpUtility.UrlDecode(url);
            url = url.TrimStart('/');
            Console.WriteLine(url);
            string[] reqParams = url.Split('/');
            int count = 0;
            string RespString = "";
            if (reqParams.Length == 1) { 
                Console.WriteLine("Requesting some - " + reqParams[0]);
                RespString = plreq.GetAudioPlaylist(reqParams[0], out count);
            }
            else if (reqParams.Length == 2)
            {
                int reqcount = 0;
                if (!int.TryParse(reqParams[1], out reqcount))
                {
                    Console.WriteLine("Pasing quantity failed, using 30 as default");
                    reqcount = 30;
                }
                Console.WriteLine("Requesting " + reqcount.ToString() + " - " + reqParams[0]);
                RespString = plreq.GetAudioPlaylist(reqParams[0], out count, reqcount);
            }
            else if (reqParams.Length == 3) {
                int reqcount = 0;
                if (!int.TryParse(reqParams[1], out reqcount))
                {
                    Console.WriteLine("Pasing quantity failed, using 30 as default");
                    reqcount = 30;
                }
                int beginOffset = 0;
                if (!int.TryParse(reqParams[2], out beginOffset))
                {
                    Console.WriteLine("Pasing offset failed, using 0 as default");
                }
                Console.WriteLine("Requesting " + reqcount.ToString() + " - " + reqParams[0] + " beginnig from #" + beginOffset);
                RespString = plreq.GetAudioPlaylist(reqParams[0], out count, reqcount, beginOffset);
            }
            else
            {
                Console.WriteLine("Parsing the request failed");
            }
            Console.WriteLine("Got " + count.ToString());
            SendString(ReqContext, RespString);
        }

        private void SendString(HttpListenerContext Context, string ResponseString) {
            byte[] ResponseBuffer = System.Text.Encoding.UTF8.GetBytes(ResponseString);
            Context.Response.ContentType = "audio/x-mpegurl";
            Context.Response.ContentLength64 = ResponseBuffer.Length;
            var OutStream = Context.Response.OutputStream;
            OutStream.Write(ResponseBuffer, 0, ResponseBuffer.Length);
            OutStream.Close();
        }
    }
}
