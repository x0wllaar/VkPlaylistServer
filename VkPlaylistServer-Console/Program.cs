using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkPlaylistServer_Console
{
    class Program
    {
        static void ShowUsage()
        {
            var name = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string[] namearr = name.Split('\\');
            name = namearr[namearr.Length - 1];
            Console.WriteLine("Usage:");
            Console.WriteLine(name + " E-mail Password Port");
            Console.WriteLine("Example " + name + " vpupkin@mail.ru qwerty123 8080");
        }

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
            Console.WriteLine("Starting to listen at port " + args[2]);
            serv.Listen();

            Console.WriteLine("Press r to restart listening, q to quit");
            Console.WriteLine("");

            ConsoleKeyInfo key;
            while (true) {
                key = Console.ReadKey(true);
                if (key.KeyChar == 'r') {
                    serv.StopListening();
                    serv.Listen();
                }
                else if (key.KeyChar == 'q') {
                    break;
                }
            }
            
            serv.StopListening();
        }

    }
}