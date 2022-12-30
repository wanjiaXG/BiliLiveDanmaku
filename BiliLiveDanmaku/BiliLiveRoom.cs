using BiliLive.Commands;
using BiliLive.Commands.Attribute;
using BiliLive.Commands.Enums;
using BiliLive.Packs;
using BiliLive.Packs.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#pragma warning disable 0067

namespace BiliLive
{
    public class BiliLiveRoom
    {
        private static readonly Dictionary<CommandType, FieldInfo> ListennerMapping;

        static BiliLiveRoom()
        {
            ListennerMapping = new Dictionary<CommandType, FieldInfo>();
            foreach (EventInfo info in typeof(BiliLiveRoom).GetEvents())
            {
                if (info.GetCustomAttribute<CommandTypeAttribute>() is CommandTypeAttribute attr)
                {
                    FieldInfo fieldInfo = typeof(BiliLiveRoom).GetField(info.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    ListennerMapping.Add(attr.Type, fieldInfo);
                }
            }
        }

        public Protocols Protocol { get; set; }

        public delegate void ConnectionEventHandler();
        public event ConnectionEventHandler Connected;
        public event ConnectionEventHandler Disconnected;

        public delegate void CommandHandler<T>(T cmd) where T : Command;

        public delegate void ConnectionFailedHandler(string message);
        public event ConnectionFailedHandler ConnectionFailed;

        public delegate void ServerHeartbeatRecievedHandler();
        public event ServerHeartbeatRecievedHandler ServerHeartbeatRecieved;

        public delegate void PopularityRecievedHandler(uint popularity);
        public event PopularityRecievedHandler PopularityRecieved;

        [CommandType(CommandType.COMBO_SEND)]
        public event CommandHandler<ComboSend> OnComboSend;

        [CommandType(CommandType.DANMU_MSG)]
        public event CommandHandler<Danmaku> OnDanmaku;

        [CommandType(CommandType.SEND_GIFT)]
        public event CommandHandler<Gift> OnGift;

        [CommandType(CommandType.GUARD_BUY)]
        public event CommandHandler<GuardBuy> OnGuardBuy;

        [CommandType(CommandType.INTERACT_WORD)]
        public event CommandHandler<InteractWord> OnInteractWord;

        [CommandType(CommandType.LIVE)]
        public event CommandHandler<Live> OnLive;

        [CommandType(CommandType.PREPARING)]
        public event CommandHandler<Preparing> OnPreparing;

        [CommandType(CommandType.ROOM_BLOCK_MSG)]
        public event CommandHandler<RoomBlock> OnRoomBlock;

        [CommandType(CommandType.SUPER_CHAT_MESSAGE)]
        public event CommandHandler<SuperChat> OnSuperChat;

        [CommandType(CommandType.WATCHED_CHANGE)]
        public event CommandHandler<WatchedChanged> OnWatchedChanged;

        [CommandType(CommandType.WELCOME)]
        public event CommandHandler<Welcome> OnWelcome;

        [CommandType(CommandType.WELCOME_GUARD)]
        public event CommandHandler<WelcomeGuard> OnWelcomeGuard;

        [CommandType(CommandType.WIDGET_BANNER)]
        public event CommandHandler<WidgetBanner> OnWidgetBanner;

        [CommandType(CommandType.STOP_LIVE_ROOM_LIST)]
        public event CommandHandler<StopLiveRoomList> OnStopLiveRoomList;

        [CommandType(CommandType.HTTP_API_ONLINE_USER)]
        public event CommandHandler<OnlineUser> OnOnlineUser;


        public event CommandHandler<Command> OnUnknow;

        public event CommandHandler<Command> OnRaw;

        private TcpClient DanmakuTcpClient { get; set; }
        private ClientWebSocket DanmakuWebSocket { get; set; }
        private BiliPackReader PackReader { get; set; }
        private BiliPackWriter PackWriter { get; set; }

        private Thread HeartbeatSenderThread { get; set; }
        private bool IsHeartbeatSenderRunning { get; set; }
        private Thread EventListenerThread { get; set; }
        public bool IsRunning { get; private set; }

        public uint RoomId { get; set; }
        public uint UID { set; get; }

        private string _cookie;
        public string Cookie
        {
            set
            {
                _cookie = value;
                UpdateCsrfToken();
            }
            get
            {
                return _cookie;
            }
        }

        public string CsrfToken { get; private set; }

        public BiliLiveRoom(uint roomId, uint uid, string cookie, Protocols protocol = Protocols.Tcp) : this(roomId, protocol)
        {
            Cookie = cookie;
            UID = uid;
        }

        public BiliLiveRoom(uint roomId, Protocols protocol = Protocols.Tcp)
        {
            IsHeartbeatSenderRunning = false;
            IsRunning = false;
            RoomId = roomId;
            Protocol = protocol;
        }

        private void UpdateCsrfToken()
        {
            //bili_jct
            if (_cookie != null)
            {
                string[] items = _cookie.Split(';');
                for (int i = 0; i < items.Length; i++)
                {
                    int index = items[i].IndexOf('=');
                    if (index > 0 && index + 1 < items[i].Length)
                    {
                        string key = items[i].Substring(0, index).Trim();
                        string value = items[i].Substring(index + 1).Trim();
                        if (key.ToLower().Equals("bili_jct"))
                        {
                            CsrfToken = value;
                            break;
                        }
                    }
                }
            }
        }

        #region Public methods

        public bool Connect()
        {
            try
            {
                DanmakuServerInfo danmakuServer = GetDanmakuServer();
                if (danmakuServer == null)
                    return false;

                switch (Protocol)
                {
                    case Protocols.Tcp:
                        
                        if(GetTcpConnection(danmakuServer) is TcpClient tcpClient && tcpClient.GetStream() is Stream stream)
                        {
                            DanmakuTcpClient = tcpClient;
                            stream.ReadTimeout = 30 * 1000 + 1000;
                            stream.WriteTimeout = 30 * 1000 + 1000;
                            PackReader = new BiliPackReader(stream);
                            PackWriter = new BiliPackWriter(stream);
                        }
                        break;
                    case Protocols.Ws:
                    case Protocols.Wss:
                        if (GetWsConnection(danmakuServer) is ClientWebSocket client)
                        {
                            PackReader = new BiliPackReader(client);
                            PackWriter = new BiliPackWriter(client);
                            DanmakuWebSocket = client;
                        }
                        break;
                }

                if (!InitConnection(danmakuServer))
                {
                    Disconnect();
                    return false;
                }

                StartEventListener();
                StartHeartbeatSender();
                Connected?.Invoke();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Disconnect()
        {
            IsRunning = false;
            StopEventListener();
            StopHeartbeatSender();
            if (DanmakuTcpClient != null)
                try { DanmakuTcpClient.Close(); } catch { }
            if (DanmakuWebSocket != null)
            {
                try { DanmakuWebSocket.Abort(); } catch { }

                try { DanmakuWebSocket.Dispose(); } catch { }
            }

            if(PackReader != null)
            {
                try { PackReader.BaseStream.Close(); } catch { }
                try { PackReader.BaseStream.Dispose(); } catch { }
            }
            Disconnected?.Invoke();
        }

        #endregion

        #region Connect to a DanmakuServer

        private TcpClient GetTcpConnection(DanmakuServerInfo danmakuServer)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(danmakuServer.Server, danmakuServer.Port);
            return tcpClient;
        }

        private ClientWebSocket GetWsConnection(DanmakuServerInfo danmakuServer)
        {
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            clientWebSocket.ConnectAsync(new Uri($"ws://{danmakuServer.Server}:{danmakuServer.WsPort}/sub"), CancellationToken.None).GetAwaiter().GetResult();
            return clientWebSocket;
        }

        private bool InitConnection(DanmakuServerInfo danmakuServer)
        {
            JToken initMsg = new JObject
            {
                ["uid"] = 0,
                ["roomid"] = GetRealRoomId(danmakuServer.RoomId),
                ["protover"] = 2,
                ["platform"] = "web",
                ["clientver"] = "1.12.0",
                ["type"] = 2,
                ["key"] = danmakuServer.Token
            };

            try
            {
                PackWriter.SendMessage((int)BiliPackWriter.MessageType.CONNECT, initMsg.ToString());
                return true;
            }
            catch
            {
                ConnectionFailed?.Invoke("连接请求发送失败");
                return false;
            }
        }

        #endregion

        #region Room info
        private long GetRealRoomId(long roomId)
        {
            try
            {
                string url = $"https://api.live.bilibili.com/room/v1/Room/room_init?id={roomId}" ;
                using (WebClient client = new WebClient())
                {
                    string result = client.DownloadString(url);
                    if(result != null)
                    {
                        Match match = Regex.Match(result, "\"room_id\":(?<RoomId>[0-9]+)");
                        if (match.Success)
                            return uint.Parse(match.Groups["RoomId"].Value);
                    }
                    throw new Exception();
                }
            }
            catch
            {
                ConnectionFailed?.Invoke("未能找到直播间");
                return -1;
            }

        }


        private DanmakuServerInfo GetDanmakuServer()
        {
            try
            {
                using(WebClient client = new WebClient())
                {
                    string url = $"https://api.live.bilibili.com/room/v1/Danmu/getConf?room_id={RoomId}";
                    JObject json = Util.GetJTokenValue<JObject>(JObject.Parse(client.DownloadString(url)), "data");
                    JObject host = Util.GetJTokenValue<JObject>(json, "host_server_list", 0);
                    DanmakuServerInfo danmakuServer = new DanmakuServerInfo
                    {
                        Uid = UID,
                        RoomId = RoomId,
                        Server = Util.GetJTokenValue<string>(host, "host"),
                        Port = Util.GetJTokenValue<int>(host, "port"),
                        WsPort = Util.GetJTokenValue<int>(host, "ws_port"),
                        WssPort = Util.GetJTokenValue<int>(host, "wss_port"),
                        Token = Util.GetJTokenValue<string>(json, "token"),
                    };

                    return danmakuServer;
                }
            }
            catch
            {
                ConnectionFailed?.Invoke("直播间信息获取失败");
                return null;
            }
        }

        #endregion

        #region Heartbeat Sender
        private void StopHeartbeatSender()
        {
            IsHeartbeatSenderRunning = false;
            if (HeartbeatSenderThread != null && HeartbeatSenderThread.IsAlive)
                try { HeartbeatSenderThread.Abort(); } catch { }
        }

        private void StartHeartbeatSender()
        {
            StopHeartbeatSender();
            HeartbeatSenderThread = new Thread(delegate ()
            {
                IsHeartbeatSenderRunning = true;
                while (IsHeartbeatSenderRunning)
                {
                    try
                    {
                        PackWriter.SendMessage((int)BiliPackWriter.MessageType.HEARTBEAT, "[object Object]");
                    }
                    catch(Exception e)
                    {
                        ConnectionFailed?.Invoke($"心跳包发送失败, {e.Message}");
                        Disconnect();
                    }
                    Thread.Sleep(30 * 1000);
                }
            });
            HeartbeatSenderThread.IsBackground = true;

            HeartbeatSenderThread.Start();
        }
        #endregion

        #region Event listener
        private void StopEventListener()
        {
            IsRunning = false;
            if (EventListenerThread != null && EventListenerThread.IsAlive)
            {
                try { EventListenerThread.Abort(); } catch { }
            }
        }

        private void StartEventListener()
        {
            EventListenerThread = new Thread(delegate ()
            {
                IsRunning = true;
                while (IsRunning)
                {
                    try
                    {
                        Pack[] packs = PackReader.ReadPacksAsync();

                        

                        if(packs == null)
                        {
                            throw new Exception("Data is null.");
                        }
                        List<Command> items = new List<Command>();

                        foreach (Pack pack in packs)
                        {
                            switch (pack.PackType)
                            {
                                case PackTypes.Popularity:
                                    if(pack is PopularityPack popularityPack)
                                    {
                                        PopularityRecieved?.Invoke(popularityPack.Popularity);
                                        OnOnlineUser?.Invoke(GetOnlineUser());
                                    }
                                    break;
                                case PackTypes.Command:
                                    
                                    

                                    if (pack is CommandPack commandPack &&
                                        commandPack.Value is JToken value
                                    ) {
                                        string cmdStr = GetOldCommand(Util.GetJTokenValue<string>(value, "cmd"));
                                        if (!string.IsNullOrWhiteSpace(cmdStr))
                                        {
                                            value["cmd"] = cmdStr;
                                        }
                                        if (BiliLiveJsonParser.Parse(value) is Command cmd)
                                        {
                                            items.Add(cmd);
                                        }
                                    }
                                    break;
                                case PackTypes.Heartbeat:
                                    ServerHeartbeatRecieved?.Invoke();
                                    break;
                            }
                        }

                        foreach (var item in items)
                        {
                            OnRaw?.Invoke(item);
                            if (item is Command cmd)
                            {
                                if (ListennerMapping.ContainsKey(item.CommandType))
                                {
                                    var info = ListennerMapping[item.CommandType];
                                    if(info.GetValue(this) is Delegate handler)
                                    {
                                        handler.DynamicInvoke(cmd);
                                    }
                                }
                                else
                                {
                                    OnUnknow?.Invoke(item);
                                }
                            }
                            
                        }
                    }
                    catch(Exception e)
                    {
                        ConnectionFailed?.Invoke($"{e.Message}. 数据读取失败, 正在断开连接...");
                        Disconnect();
                    }
                }
            });
            EventListenerThread.IsBackground = true;
            EventListenerThread.Start();
        }

        private string GetOldCommand(string v)
        {
            Regex regex = new Regex("[1-9a-zA-Z_]{1,999}");
            Match match = regex.Match(v);
            if (match.Success)
            {
                return match.Value;
            }
            return string.Empty;
        }

        private OnlineUser GetOnlineUser()
        {
            return OnlineUser.NewInstance(Cookie, RoomId, UID);
        }

        #endregion

        #region Message Sender

        public class Result
        {
            public bool Success { get; }

            public string Message { get; }

            public Result(bool success, string message)
            {
                Success = success;
                Message = message;
            }
        }

        public Result Send(string msg)
        {
            try
            {
                if (msg.Length > 20)
                {
                    return new Result(false, $"弹幕消息: \" {msg}\"超出限制长度, 请控制在20个字以内");
                }

                if (string.IsNullOrWhiteSpace(Cookie))
                {
                    return new Result(false, "账号未登录, 无法发送弹幕消息到直播间!");
                }

                string url = "https://api.live.bilibili.com/msg/send";

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.Host = "api.live.bilibili.com";
                request.KeepAlive = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36";
                request.ContentType = "multipart/form-data; boundary=----WebKitFormBoundaryQJAfXGclnyMx6OX6";
                request.Accept = "*/*";
                request.Referer = "https://live.bilibili.com/";
                request.Headers["Cookie"] = Cookie;
                request.Headers["Origin"] = "https://live.bilibili.com";

                StringBuilder sb = new StringBuilder();
                sb.Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6\r\n")
                    .Append("Content-Disposition: form-data; name=\"bubble\"\r\n\r\n0\r\n")
                    .Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6\r\n")
                    .Append($"Content-Disposition: form-data; name=\"msg\"\r\n\r\n{msg}\r\n")
                    .Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6\r\n")
                    .Append("Content-Disposition: form-data; name=\"color\"\r\n\r\n16777215\r\n")
                    .Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6\r\n")
                    .Append("Content-Disposition: form-data; name=\"mode\"\r\n\r\n1\r\n")
                    .Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6\r\n")
                    .Append("Content-Disposition: form-data; name=\"fontsize\"\r\n\r\n25\r\n")

                    .Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6\r\n")
                    .Append("Content-Disposition: form-data; name=\"rnd\"\r\n\r\n1633713628\r\n")

                    .Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6\r\n")
                    .Append($"Content-Disposition: form-data; name=\"roomid\"\r\n\r\n{RoomId}\r\n")


                    .Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6\r\n")
                    .Append($"Content-Disposition: form-data; name=\"csrf\"\r\n\r\n{CsrfToken}\r\n")


                    .Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6\r\n")
                    .Append($"Content-Disposition: form-data; name=\"csrf_token\"\r\n\r\n{CsrfToken}\r\n")
                    .Append("------WebKitFormBoundaryQJAfXGclnyMx6OX6--\r\n");

                byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());

                request.ContentLength = buffer.Length;

                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                request.GetRequestStream().Flush();

                StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8);
                string result = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();

                JObject json = JObject.Parse(result);

                switch (Util.GetJTokenValue<int>(json, "code"))
                {
                    case 0:
                        if (!string.IsNullOrWhiteSpace(Util.GetJTokenValue<string>(json, "message")))
                        {
                            return new Result(false, $"弹幕消息: \" {msg}\"发送失败，请重新输入");
                        }
                        break;
                    case 10030:
                    case 10031:
                        if (!string.IsNullOrWhiteSpace(Util.GetJTokenValue<string>(json, "message")))
                        {
                            return new Result(false, $"弹幕消息: \" {msg}\"发送失败，您发送弹幕的频率过快");
                        }
                        break;
                }

                return new Result(true, "OK");
            }
            catch(Exception ex)
            {
                return new Result(false, ex.Message);
            }
        }

        public Result ChangeRoomName(string name)
        {
            try
            {
                if (name.Length > 20)
                {
                    return new Result(false, $"超出限制长度, 请将直播间名称控制在20个字以内");
                }

                if (string.IsNullOrWhiteSpace(Cookie))
                {
                    return new Result(false, "账号未登录, 无法发送弹幕消息到直播间!");
                }

                string url = "https://api.live.bilibili.com/room/v1/Room/update";

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.Host = "api.live.bilibili.com";
                request.KeepAlive = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.71 Safari/537.36";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "*/*";
                request.Referer = "https://link.bilibili.com/p/center/index";
                request.Headers["Cookie"] = Cookie;
                request.Headers["Origin"] = "https://link.bilibili.com";

                string PostContent = $"room_id={RoomId}&title={name}&csrf_token={CsrfToken}&csrf={CsrfToken}";

                byte[] buffer = Encoding.UTF8.GetBytes(PostContent);

                request.ContentLength = buffer.Length;

                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                request.GetRequestStream().Flush();

                StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8);
                string result = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                JObject json = JObject.Parse(result);

                switch (Util.GetJTokenValue<int>(json, "code"))
                {
                    case 0:
                        return new Result(true, "OK");
                    default:
                        return new Result(false, Util.GetJTokenValue<string>(json, "msg"));
                }
                
                
            }
            catch (Exception ex)
            {
                return new Result(false, ex.Message);
            }
        }

        #endregion
    }
}
