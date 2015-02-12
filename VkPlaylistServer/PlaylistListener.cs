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
            string[] reqParams = url.Split('/');
            
            int count = 0;
            int reqcount = 30;
            int beginOffset = 0;
            string RespString = "";
            
            try { 
                RespString = plreq.GetAudioPlaylist(reqParams[0], out count);
            }catch{
                Console.WriteLine("Parsing the request failed");
                SendString(ReqContext, "");
                return;   
            }
            
            try {
                reqcount = int.Parse(reqParams[1]);
            }catch{
                Console.WriteLine("Pasing quantity failed, using 30 as default");
            }

            try {
                beginOffset = int.Parse(reqParams[2]);
            }
            catch {
                Console.WriteLine("Pasing offset failed, using 0 as default");    
            }

            Console.WriteLine("Requesting " + reqcount.ToString() + " - " + reqParams[0] + " beginning from #" + beginOffset);
            RespString = plreq.GetAudioPlaylist(reqParams[0], out count, reqcount, beginOffset);
            
            Console.WriteLine("Got " + count.ToString());
            Console.WriteLine("");
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
