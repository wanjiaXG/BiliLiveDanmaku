using BiliLive.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BiliLive
{
    public class BiliLiveListener
    {
        public Protocols Protocol { get; set; }

        public delegate void ConnectionEventHandler();
        public event ConnectionEventHandler Connected;
        public event ConnectionEventHandler Disconnected;

        public delegate void ConnectionFailedHandler(string message);
        public event ConnectionFailedHandler ConnectionFailed;

        public delegate void ServerHeartbeatRecievedHandler();
        public event ServerHeartbeatRecievedHandler ServerHeartbeatRecieved;

        public delegate void PopularityRecievedHandler(uint popularity);
        public event PopularityRecievedHandler PopularityRecieved;

        public delegate void ComboSendHandler(ComboSend cmd);
        public event ComboSendHandler OnComboSend;

        public delegate void DanmakuHandler(Danmaku cmd);
        public event DanmakuHandler OnDamaku;

        public delegate void GiftHandler(Gift cmd);
        public event GiftHandler OnGift;

        public delegate void GuardBuyHandler(GuardBuy cmd);
        public event GuardBuyHandler OnGuardBuy;

        public delegate void InteractWordHandler(InteractWord cmd);
        public event InteractWordHandler OnInteractWord;

        public delegate void LiveHandler(Live cmd);
        public event LiveHandler OnLive;

        public delegate void PreparingHandler(Preparing cmd);
        public event PreparingHandler OnPreparing;

        public delegate void RawHandler(IData cmd);
        public event RawHandler OnRaw;

        public delegate void RoomBlockHandler(RoomBlock cmd);
        public event RoomBlockHandler OnRoomBlock;

        public delegate void SuperChatHandler(SuperChat cmd);
        public event SuperChatHandler OnSuperChat;

        public delegate void WatchedChangedHandler(WatchedChanged cmd);
        public event WatchedChangedHandler OnWatchedChanged;

        public delegate void WelcomeHandler(Welcome cmd);
        public event WelcomeHandler OnWelcome;

        public delegate void WelcomeGuardHandler(WelcomeGuard cmd);
        public event WelcomeGuardHandler OnWelcomeGuard;

        public delegate void UnknowGuardHandler(IData cmd);
        public event UnknowGuardHandler OnUnknow;


        private TcpClient DanmakuTcpClient { get; set; }
        private ClientWebSocket DanmakuWebSocket { get; set; }
        private uint RoomId { get; set; }

        private BiliPackReader PackReader { get; set; }
        private BiliPackWriter PackWriter { get; set; }

        private Thread HeartbeatSenderThread { get; set; }
        private bool IsHeartbeatSenderRunning { get; set; }

        private Thread EventListenerThread { get; set; }
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="roomId"></param>

        public BiliLiveListener(uint roomId, Protocols protocol = Protocols.Tcp)
        {
            IsHeartbeatSenderRunning = false;
            IsRunning = false;
            RoomId = roomId;
            Protocol = protocol;
        }
        
        #region Public methods

        public Task<bool> ConnectAsync() => new Task<bool>(Connect);

        public bool Connect()
        {
            try
            {
                DanmakuServer danmakuServer = GetDanmakuServer(RoomId);
                if (danmakuServer == null)
                    return false;

                switch (Protocol)
                {
                    case Protocols.Tcp:
                        DanmakuTcpClient = GetTcpConnection(danmakuServer);
                        Stream stream = DanmakuTcpClient.GetStream();

                        stream.ReadTimeout = 30 * 1000 + 1000;
                        stream.WriteTimeout = 30 * 1000 + 1000;

                        PackReader = new BiliPackReader(stream);
                        PackWriter = new BiliPackWriter(stream);
                        break;
                    case Protocols.Ws:
                        DanmakuWebSocket = GetWsConnection(danmakuServer);
                        PackReader = new BiliPackReader(DanmakuWebSocket);
                        PackWriter = new BiliPackWriter(DanmakuWebSocket);
                        break;
                    case Protocols.Wss:
                        DanmakuWebSocket = GetWssConnection(danmakuServer);
                        PackReader = new BiliPackReader(DanmakuWebSocket);
                        PackWriter = new BiliPackWriter(DanmakuWebSocket);
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

        public Task DisconnectAsync() => new Task(Disconnect);

        public void Disconnect()
        {
            StopEventListener();
            StopHeartbeatSender();
            if (DanmakuTcpClient != null)
                try { DanmakuTcpClient.Close(); } catch { }
            if (DanmakuWebSocket != null)
            {
                try { DanmakuWebSocket.Abort(); } catch { }

                try { DanmakuWebSocket.Dispose(); } catch { }
                  
            }
            Disconnected?.Invoke();
        }

        #endregion

        #region Connect to a DanmakuServer

        private TcpClient GetTcpConnection(DanmakuServer danmakuServer)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(danmakuServer.Server, danmakuServer.Port);
            return tcpClient;
        }

        private ClientWebSocket GetWsConnection(DanmakuServer danmakuServer)
        {
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            clientWebSocket.ConnectAsync(new Uri($"ws://{danmakuServer.Server}:{danmakuServer.WsPort}/sub"), CancellationToken.None).GetAwaiter().GetResult();
            return clientWebSocket;
        }

        private ClientWebSocket GetWssConnection(DanmakuServer danmakuServer)
        {
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            clientWebSocket.ConnectAsync(new Uri($"wss://{danmakuServer.Server}:{danmakuServer.WssPort}/sub"), CancellationToken.None).GetAwaiter().GetResult();
            return clientWebSocket;
        }

        private bool InitConnection(DanmakuServer danmakuServer)
        {
            JToken initMsg = new JObject
            {
                ["uid"] = 0,
                ["roomid"] = danmakuServer.RoomId,
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
            catch (SocketException)
            {
                ConnectionFailed?.Invoke("连接请求发送失败");
                return false;
            }
            catch (InvalidOperationException)
            {
                ConnectionFailed?.Invoke("连接请求发送失败");
                return false;
            }
            catch (IOException)
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
                string url = "https://api.live.bilibili.com/room/v1/Room/room_init?id=" + roomId;
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

        private DanmakuServer GetDanmakuServer(long roomId)
        {
            roomId = GetRealRoomId(roomId);
            if (roomId < 0)
            {
                return null;
            }
            try
            {
                using(WebClient client = new WebClient())
                {
                    string url = "https://api.live.bilibili.com/room/v1/Danmu/getConf?room_id=" + roomId;
                    string result = client.DownloadString(url);
                    JObject json = JObject.Parse(result);
                    if (json!=null && json.ContainsKey("code") && int.Parse(json["code"].ToString()) != 0)
                    {
                        return null;
                    }

                    DanmakuServer danmakuServer = new DanmakuServer
                    {
                        RoomId = roomId,
                        Server = json["data"]["host_server_list"][0]["host"].ToString(),
                        Port = int.Parse(json["data"]["host_server_list"][0]["port"].ToString()),
                        WsPort = int.Parse(json["data"]["host_server_list"][0]["ws_port"].ToString()),
                        WssPort = int.Parse(json["data"]["host_server_list"][0]["wss_port"].ToString()),
                        Token = json["data"]["token"].ToString()
                    };

                    return danmakuServer;
                }
            }
            catch (WebException)
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
                    catch (SocketException)
                    {
                        ConnectionFailed?.Invoke("心跳包发送失败");
                        Disconnect();
                    }
                    catch (InvalidOperationException)
                    {
                        ConnectionFailed?.Invoke("心跳包发送失败");
                        Disconnect();
                    }
                    catch (IOException)
                    {
                        ConnectionFailed?.Invoke("心跳包发送失败");
                        Disconnect();
                    }
                    Thread.Sleep(30 * 1000);
                }
            });
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
                        BiliPackReader.IPack[] packs = PackReader.ReadPacksAsync();

                        List<IData> items = new List<IData>();

                        foreach (BiliPackReader.IPack pack in packs)
                        {
                            switch (pack.PackType)
                            {
                                case BiliPackReader.PackTypes.Popularity:
                                    PopularityRecieved?.Invoke(((BiliPackReader.PopularityPack)pack).Popularity);
                                    break;
                                case BiliPackReader.PackTypes.Command:
                                    JToken value = ((BiliPackReader.CommandPack)pack).Value;
                                    IData item = BiliLiveJsonParser.Parse(value);
                                    if (item != null)
                                        items.Add(item);
                                    break;
                                case BiliPackReader.PackTypes.Heartbeat:
                                    ServerHeartbeatRecieved?.Invoke();
                                    break;
                            }
                        }

                        foreach (var item in items)
                        {
                            OnRaw?.Invoke(item);

                            if (item is ICommand cmd)
                            {
                                switch (cmd.CommandType)
                                {
                                    case CommandType.COMBO_SEND:
                                        OnComboSend?.Invoke(item as ComboSend);
                                        break;

                                    case CommandType.DANMU_MSG:
                                        OnDamaku?.Invoke(item as Danmaku);
                                        break;

                                    case CommandType.SEND_GIFT:
                                        OnGift?.Invoke(item as Gift);
                                        break;

                                    case CommandType.GUARD_BUY:
                                        OnGuardBuy?.Invoke(item as GuardBuy);
                                        break;

                                    case CommandType.INTERACT_WORD:
                                        OnInteractWord?.Invoke(item as InteractWord);
                                        break;

                                    case CommandType.LIVE:
                                        OnLive?.Invoke(item as Live);
                                        break;

                                    case CommandType.PREPARING:
                                        OnPreparing?.Invoke(item as Preparing);
                                        break;

                                    case CommandType.ROOM_BLOCK_MSG:
                                        OnRoomBlock?.Invoke(item as RoomBlock);
                                        break;

                                    case CommandType.SUPER_CHAT_MESSAGE:
                                        OnSuperChat?.Invoke(item as SuperChat);
                                        break;

                                    case CommandType.WATCHED_CHANGE:
                                        OnWatchedChanged?.Invoke(item as WatchedChanged);
                                        break;

                                    case CommandType.WELCOME:
                                        OnWelcome?.Invoke(item as Welcome);
                                        break;

                                    case CommandType.WELCOME_GUARD:
                                        OnWelcomeGuard?.Invoke(item as WelcomeGuard);
                                        break;

                                    default:
                                        OnUnknow?.Invoke(item as Unknow);
                                        break;
                                }
                            }



                        }
                    }
                    catch (SocketException)
                    {
                        ConnectionFailed?.Invoke("数据读取失败");
                        Disconnect();
                    }
                    catch (IOException)
                    {
                        ConnectionFailed?.Invoke("数据读取失败");
                        Disconnect();
                    }
                }
            })
            {

            };
            EventListenerThread.Start();
        }

        #endregion
    }
}
