using BiliLive;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BiliLive.BiliLiveJsonParser;

namespace BiliBIliDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            uint roomId = 189205;
            BiliLiveListener listener = new BiliLiveListener(roomId, BiliLiveListener.Protocols.Tcp);
            listener.Connected += Connected;
            listener.ConnectionFailed += ConnectionFailed;
            listener.Disconnected += Disconnected;
            listener.ItemsRecieved += ItemsRecieved;
            listener.JsonsRecieved += JsonsRecieved;
            listener.PopularityRecieved += PopularityRecieved;
            listener.ServerHeartbeatRecieved += ServerHeartbeatRecieved;
            listener.Connect();
            while (true)
            {
                string cmd = Console.ReadLine();
                if ("exit".Equals(cmd))
                {
                    break;
                }
            }
        }

        public static string GetTime()
        {
            return $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]";
        }

        private static void ServerHeartbeatRecieved()
        {
            Console.WriteLine("ServerHeartbeatRecieved");
            Append("ServerHeartbeatRecieved");
        }

        static void Append(object obj)
        {
            string msg = $"{GetTime()}: {obj.ToString()}\r\n";
            File.AppendAllText("log.txt", msg);
        }

        private static void PopularityRecieved(uint popularity)
        {
            Console.WriteLine($"Popularity: {popularity}");
            Append($"Popularity: {popularity}");
        }

        private static void JsonsRecieved(JToken jsons)
        {
            Console.WriteLine("JsonsRecieved: " + jsons);
            Append("JsonsRecieved: " + jsons);
        }

        private static void ItemsRecieved(IItem item)
        {
            Console.WriteLine("ItemsRecieved CMD: " + item.Cmd);
            Append("ItemsRecieved CMD: " + item.Cmd);
        }

        private static void Disconnected()
        {
            Console.WriteLine("Disconnected");
            Append("Disconnected");
        }

        private static void ConnectionFailed(string message)
        {
            Console.WriteLine($"ConnectionFailed: {message}");
            Append($"ConnectionFailed: {message}");
        }

        private static void Connected()
        {
            Console.WriteLine("Connected");
            Append("Connected");
        }
    }
}
