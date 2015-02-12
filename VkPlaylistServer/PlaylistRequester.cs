using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkPlaylistServer
{
    public class PlaylistRequester
    {
        const int appid = 4777065;
        private VkNet.VkApi vkapi = new VkNet.VkApi();

        public PlaylistRequester(string uname, string password){ 
            vkapi.Authorize(appid, uname, password, VkNet.Enums.Filters.Settings.Audio);
        }

        public String GetAudioPlaylist(String query, out int gotcount, int count = 30, int beginOffset = 0) {
            int AudioCount;
            StringBuilder Playlist = new StringBuilder();
            Playlist.AppendLine("#EXTM3U");
            Playlist.AppendLine("");
            List<VkNet.Model.Attachments.Audio> audiolist = new List<VkNet.Model.Attachments.Audio>();

            int rem;
            int offset = beginOffset;
            Math.DivRem((int)count, 200, out rem);
            var audios = vkapi.Audio.Search(query, out AudioCount, true, null, null, rem, offset);
            audios.ToList().ForEach(recording => audiolist.Add(recording));
            count -= rem;
            offset += rem;
            for(;count > 0; count -= 200){
                audios = vkapi.Audio.Search(query, out AudioCount, true, null, null, 200, offset);
                audios.ToList().ForEach(recording => audiolist.Add(recording));
                if (AudioCount == 0) { break; }
                offset += 200;
            }
            
            audiolist.ForEach(recording =>
            {
                StringBuilder PlaylistEntry = new StringBuilder();
                PlaylistEntry.Append("#EXTINF:");
                PlaylistEntry.Append(recording.Duration);
                PlaylistEntry.Append(",");
                PlaylistEntry.Append(recording.Artist);
                PlaylistEntry.Append(" - ");
                PlaylistEntry.Append(recording.Title);
                PlaylistEntry.AppendLine("");
                PlaylistEntry.AppendLine(recording.Url.ToString());
                PlaylistEntry.AppendLine("");
                Playlist.Append(PlaylistEntry);
            });

            gotcount = audiolist.Count;
            return Playlist.ToString();
        }
       
    }
}
