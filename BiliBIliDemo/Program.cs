using BiliLive;
using BiliLive.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace BiliBIliDemo
{
    class Program
    {
        static BiliLiveRoom listener;
        static void Main(string[] args)
        {
            uint roomId = 189205;// 86002;// 9758780;// 5252;//22508204;
            string cookie = File.ReadAllText("D://cookie.txt");
            listener = new BiliLiveRoom(roomId, 826842, cookie);
            listener.Connected += Connected;
            listener.ConnectionFailed += ConnectionFailed;
            listener.Disconnected += Disconnected;
            listener.PopularityRecieved += PopularityRecieved;
            listener.ServerHeartbeatRecieved += ServerHeartbeatRecieved;
            //listener.OnRaw += OnRaw;
            //listener.OnComboSend += OnComboSend;
            listener.OnComboSend += OnComboSend02;

            //listener.OnDanmaku += OnDamaku;
            listener.OnDanmaku += OnDamaku02;
            //listener.OnGift += OnGift;
            //listener.OnGuardBuy += OnGuardBuy;
            //listener.OnInteractWord += OnInteractWord;
            //listener.OnLive += OnLive;
            //listener.OnPreparing += OnPreparing;
            //listener.OnRoomBlock += OnRoomBlock;
            //listener.OnSuperChat += OnSuperChat;
            //listener.OnUnknow += OnUnknow;
            //listener.OnWatchedChanged += OnWatchedChanged;
            //listener.OnWelcome += OnWelcome;
            //listener.OnWelcomeGuard += OnWelcomeGuard;

            listener.OnWidgetBanner += OnWidgetBanner;

            listener.OnOnlineUser += OnOnlineUser;

            listener.Connect();
        }


        static int i = 10;
        private static void OnOnlineUser(OnlineUser cmd)
        {
            Console.WriteLine("在线人数:" + cmd.Count + ", 成员:" + GetNames(cmd.Users));
            var msg = listener.Send("在线人数:" + cmd.Count);
            Console.WriteLine(msg.Message);
            msg = listener.ChangeRoomName("TEST" + (i++));
            Console.WriteLine(msg.Message);
        }

        private static string GetNames(OnlineUser.User[] users)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var item in users)
            {
                sb.Append(item.Username).Append(",");
            }
            if(sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        private static void OnWidgetBanner(WidgetBanner cmd)
        {
            WidgetBanner.Widget widget = cmd.WidgetList[0];
            Console.WriteLine(widget.Title);
        }

        private static void OnDamaku02(Danmaku cmd)
        {
            Console.WriteLine(cmd.Username + ": " + cmd.Message);
        }

        private static void OnComboSend02(ComboSend cmd)
        {
            Console.WriteLine(cmd.Username  + " " + cmd.Action);
        }

        private static void OnWelcomeGuard(WelcomeGuard cmd)
        {
            string msg = $"OnWelcomeGuard:\r\n" +
                $"UID: {cmd.UID}\r\n" +
                $"Name: {cmd.Username}\r\n" +
                $"GuardLevel: {cmd.GuardLevel}\r\n" +
                $"DateTime: {cmd.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnWelcome(Welcome cmd)
        {
            string msg = $"OnWelcome:\r\n" +
                $"UID: {cmd.UID}\r\n" +
                $"Name: {cmd.Username}\r\n" +
                $"IsSVIP: {cmd.Svip}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnWatchedChanged(WatchedChanged cmd)
        {
            string msg = $"OnWatchedChanged:\r\n" +
                $"WatchedCount: {cmd.Count}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnUnknow(Command data)
        {
            string msg = $"OnUnknow:\r\n" +
                $"Data: {data.RawData}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnSuperChat(SuperChat cmd)
        {
            string msg = $"OnSuperChat:\r\n" +
                $"UID: {cmd.UID}\r\n" +
                $"Name: {cmd.Username}\r\n" +
                $"Face: {cmd.Face}\r\n" +
                $"TransMark: {cmd.TransMark}\r\n" +
                $"Message: {cmd.Message}\r\n" +
                $"MessageTrans: {cmd.MessageTrans}\r\n" +
                $"Price: {cmd.Price}\r\n" +
                $"Duration: {cmd.Duration.TotalSeconds}s\r\n";// +
                //$"DateTime: {cmd.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnRoomBlock(RoomBlock cmd)
        {
            string msg = $"OnRoomBlock:\r\n" +
                $"UID: {cmd.UID}\r\n" +
                $"Name: {cmd.Username}\r\n" +
                $"Operator: {cmd.Operator}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnPreparing(Preparing cmd)
        {
            string msg = $"OnPreparing:\r\n" +
                $"TransMark: {cmd.RoomId}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnLive(Live cmd)
        {
            string msg = $"OnLive:\r\n" +
                $"LiveKey: {cmd.LiveKey}\r\n" +
                $"LiveModel: {cmd.LiveModel}\r\n" +
                $"LivePlatform: {cmd.LivePlatform}\r\n" +
                $"RoomId: {cmd.RoomId}\r\n" +
                $"SubSessionKey: {cmd.SubSessionKey}\r\n" +
                $"VoiceBackground: {cmd.VoiceBackground}\r\n";// +
                //$"DateTime: {cmd.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnInteractWord(InteractWord cmd)
        {
            string msg = $"OnInteractWord:\r\n" +
                $"UID: {cmd.UID}\r\n" +
                $"Name: {cmd.Username}\r\n" +
                $"Identity: {GetIndentity(cmd.Identity)}\r\n" +
                $"MessageType: {cmd.MessageType}\r\n";// +
                //$"DateTime: {cmd.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static object GetIndentity(ICollection<InteractWord.Identities> identity)
        {
            StringBuilder sb = new StringBuilder();
            foreach(InteractWord.Identities item in identity)
            {
                sb.Append(item.ToString()).Append(",");
            }
            if (sb.Length == 0) return sb.ToString();
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        private static void OnGuardBuy(GuardBuy cmd)
        {
            string msg = $"OnGuardBuy:\r\n" +
                $"UID: {cmd.UID}\r\n" +
                $"Name: {cmd.Username}\r\n" +
                $"GiftName: {cmd.GiftName}\r\n" +
                $"GuardLevel: {cmd.GuardLevel}\r\n";// +
                //$"DateTime: {cmd.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnGift(Gift cmd)
        {
            string msg = $"OnGift:\r\n" +
                $"UID: {cmd.UID}\r\n" +
                $"Name: {cmd.Username}\r\n" +
                $"GiftId: {cmd.GiftId}\r\n" +
                $"GiftName: {cmd.GiftName}\r\n" +
                $"Number: {cmd.Number}\r\n" +
                $"Action: {cmd.Action}\r\n" +
                $"CoinType: {cmd.CoinType}\r\n" +
                $"FaceUri: {cmd.FaceUri}\r\n";// +
                //$"DateTime: {cmd.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnDamaku(Danmaku cmd)
        {
            string msg = $"OnDamaku:\r\n" +
                $"UID: {cmd.UID}\r\n" +
                $"Name: {cmd.Username}\r\n" +
                $"Message: {cmd.Message}\r\n";
               // $"DateTime: {cmd.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnComboSend(ComboSend cmd)
        {
            string msg = $"OnComboSend:\r\n" +
                $"UID: {cmd.UID}\r\n" +
                $"Name: {cmd.Username}\r\n" +
                $"Action: {cmd.Action}\r\n" +
                $"GiftId: {cmd.GiftId}\r\n" +
                $"GiftName: {cmd.GiftName}\r\n" +
                $"Number: {cmd.TotalNumber}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        private static void OnRaw(Command data)
        {
            string msg = $"OnRaw:\r\n" +
                $"Data: {data.RawData}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        public static string GetTime()
        {
            return $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]";
        }

        private static void ServerHeartbeatRecieved()
        {
            string msg = $"ServerHeartbeatRecieved\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }

        static void Append(object obj)
        {
            lock (listener)
            {
                string msg = $"{GetTime()}: {obj.ToString()}\r\n";
                File.AppendAllText("log.txt", msg);
            }
        }

        private static void PopularityRecieved(uint popularity)
        {
            string msg = $"PopularityRecieved:\r\n" +
                $"Popularity: {popularity}\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }
        private static void Disconnected()
        {
            string msg = $"Disconnected\r\n";
            Console.WriteLine(msg);
            Append(msg);
            ReConnect();



        }


        private static void ReConnect()
        {
            new Thread(() =>
            {
                Thread.Sleep(10 * 1000);
                lock (listener)
                {
                    if (!listener.IsRunning)
                    {
                        string msg = "正在重连...";
                        Console.WriteLine(msg);
                        listener?.Connect();
                        Append(msg);
                    }
                    else
                    {
                        Console.WriteLine("Listenner正在运行，无需重连");
                    }
                }
            })
            { IsBackground = true }.Start();
            
        }

        private static void ConnectionFailed(string message)
        {
            string msg = $"ConnectionFailed\r\n";
            Console.WriteLine(msg);
            Append(msg);
            ReConnect();
        }

        private static void Connected()
        {
            string msg = $"Connected\r\n";
            Console.WriteLine(msg);
            Append(msg);
        }
    }
}
