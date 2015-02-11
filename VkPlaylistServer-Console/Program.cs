using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkPlaylistServer_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3) {
                ShowUsage();
                return;
            }

            int port;
            if (!int.TryParse(args[2], out port)) {
                Console.WriteLine("Failed to parse port");
                ShowUsage();
                return;
            }

            var serv = new VkPlaylistServer.PlaylistListener(args[0], args[1], port);
            serv.Listen();
            Console.Clear();
            Console.ReadKey();
            serv.StopListening();
        }

        static void ShowUsage() {
            var name = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Console.WriteLine("Usage:");
            Console.WriteLine(name + " E-mail Password Port");
            Console.WriteLine("Example " + name + "vpupkin@mail.ru qwerty123 8080");
        }
    }
}