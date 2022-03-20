using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BiliLive
{
    public class BiliLiveJsonParser
    {
        public enum Cmds
        {
            UNKNOW,
            LIVE,
            PREPARING,
            DANMU_MSG,
            SEND_GIFT,
            SPECIAL_GIFT,
            USER_TOAST_MSG,
            GUARD_MSG,
            GUARD_BUY,
            GUARD_LOTTERY_START,
            WELCOME,
            WELCOME_GUARD,
            ENTRY_EFFECT,
            SYS_MSG,
            ROOM_BLOCK_MSG,
            COMBO_SEND,
            ROOM_RANK,
            TV_START,
            NOTICE_MSG,
            SYS_GIFT,
            ROOM_REAL_TIME_MESSAGE_UPDATE,
            SUPER_CHAT_ENTRANCE,
            SUPER_CHAT_MESSAGE,
            SUPER_CHAT_MESSAGE_DELETE,
            INTERACT_WORD
        }

        [Serializable]
        public class User
        {
            public uint Id;
            public string Name;

            public User(uint id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        public interface IItem
        {
            Cmds Cmd { get; }
        }

        public interface ITimeStampedItem : IItem
        {
            DateTime TimeStamp { get; }
        }

        [Serializable]
        public class Raw : IItem
        {
            public Cmds Cmd { get; }
            public JToken Value { get; }

            public Raw(Cmds cmd, JToken value)
            {
                Cmd = cmd;
                Value = value;
            }
        }

        [Serializable]
        public class Danmaku : ITimeStampedItem
        {
            public Cmds Cmd => Cmds.DANMU_MSG;
            public DateTime TimeStamp { get; private set; }

            public User Sender { get; private set; }
            public string Message { get; private set; }
            public uint Type { get; private set; }

            public Danmaku(JToken json)
            {
                Sender = new User((uint)json["info"][2][0], Regex.Unescape(json["info"][2][1].ToString()));
                try
                {
                    Message = Regex.Unescape(json["info"][1].ToString());
                }
                catch (Exception)
                {
                    Message = json["info"][1].ToString();
                }

                Type = (uint)json["info"][0][9];
                TimeStamp = new DateTime(1970, 01, 01).AddMilliseconds(double.Parse(json["info"][0][4].ToString()));
            }
        }

        [Serializable]
        public class SuperChat : ITimeStampedItem
        {
            public Cmds Cmd => Cmds.SUPER_CHAT_MESSAGE;
            public DateTime TimeStamp { get; private set; }

            public uint Price { get; private set; }
            public string Message { get; private set; }
            public bool TransMark { get; private set; }
            public string MessageTrans { get; private set; }
            public User User { get; private set; }
            public string Face { get; private set; }
            public TimeSpan Duration { get; private set; }

            public SuperChat(JToken json)
            {
                TimeStamp = new DateTime(1970, 01, 01).AddMilliseconds(double.Parse(json["data"]["ts"].ToString()));

                Price = uint.Parse(json["data"]["price"].ToString());
                try
                {
                    Message = Regex.Unescape(json["data"]["message"].ToString());
                }
                catch (Exception)
                {
                    Message = json["data"]["message"].ToString();
                }
                TransMark = int.Parse(json["data"]["trans_mark"].ToString()) != 0;
                MessageTrans = json["data"]["message_trans"].ToString();
                User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["user_info"]["uname"].ToString()));
                Face = json["data"]["user_info"]["face"].ToString();
                Duration = TimeSpan.FromSeconds(double.Parse(json["data"]["time"].ToString()));
            }
        }

        [Serializable]
        public class Gift : ITimeStampedItem
        {
            public Cmds Cmd => Cmds.SEND_GIFT;
            public DateTime TimeStamp { get; private set; }

            public string GiftName { get; private set; }
            public uint Number { get; private set; }
            public User Sender { get; private set; }
            public string FaceUri { get; private set; }
            public uint GiftId { get; private set; }
            public string Action { get; private set; }
            public string CoinType { get; private set; }

            public Gift(JToken json)
            {
                GiftName = Regex.Unescape(json["data"]["giftName"].ToString());
                Number = uint.Parse(json["data"]["num"].ToString());
                Sender = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["uname"].ToString()));
                FaceUri = json["data"]["face"].ToString();
                GiftId = uint.Parse(json["data"]["giftId"].ToString());
                Action = json["data"]["action"].ToString();
                CoinType = json["data"]["coin_type"].ToString();

                TimeStamp = new DateTime(1970, 01, 01).AddSeconds(double.Parse(json["data"]["timestamp"].ToString()));
            }
        }

        [Serializable]
        public class ComboSend : IItem
        {
            public Cmds Cmd => Cmds.COMBO_SEND;

            public User Sender { get; private set; }
            public string GiftName { get; private set; }
            public uint Number { get; private set; }
            public uint GiftId { get; private set; }
            public string Action { get; private set; }

            public ComboSend(JToken json)
            {
                Sender = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["uname"].ToString()));
                GiftName = Regex.Unescape(json["data"]["gift_name"].ToString());
                Number = uint.Parse(json["data"]["total_num"].ToString());
                GiftId = uint.Parse(json["data"]["gift_id"].ToString());
                Action = json["data"]["action"].ToString();
            }
        }

        [Serializable]
        public class Welcome : IItem
        {
            public Cmds Cmd => Cmds.WELCOME;

            public User User { get; private set; }
            public bool Svip { get; private set; }

            public Welcome(JToken json)
            {
                User = new User( uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["uname"].ToString()));
                Svip = int.Parse(json["data"]["svip"].ToString()) != 0;
            }
        }

        [Serializable]
        public class WelcomeGuard : IItem
        {
            public Cmds Cmd => Cmds.WELCOME_GUARD;

            public User User { get; private set; }
            public uint GuardLevel { get; private set; }

            public WelcomeGuard(JToken json)
            {
                User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["username"].ToString()));
                GuardLevel = uint.Parse(json["data"]["guard_level"].ToString());
            }
        }

        [Serializable]
        public class InteractWord : ITimeStampedItem
        {
            public enum Identities
            {
                Unknown = 0,
                Normal,
                Manager,
                Fans,
                Vip,
                SVip,
                GuardJian,
                GuardTi,
                GuardZong
            }

            public enum MessageTypes
            {
                Unknown = 0,
                Entry,
                Attention,
                Share,
                SpecialAttention,
                MutualAttention
            }

            public Cmds Cmd => Cmds.INTERACT_WORD;
            public DateTime TimeStamp { get; private set; }

            public User User { get; private set; }
            public ICollection<Identities> Identity { get; private set; }
            public MessageTypes MessageType { get; private set; }

            public InteractWord(JToken json)
            {
                User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["uname"].ToString()));

                List<Identities> identities = new List<Identities>();
                foreach (JToken i in json["data"]["identities"])
                {
                    switch (int.Parse(i.ToString()))
                    {
                        case 1:
                            identities.Add(Identities.Normal);
                            break;
                        case 2:
                            identities.Add(Identities.Manager);
                            break;
                        case 3:
                            identities.Add(Identities.Fans);
                            break;
                        case 4:
                            identities.Add(Identities.Vip);
                            break;
                        case 5:
                            identities.Add(Identities.SVip);
                            break;
                        case 6:
                            identities.Add(Identities.GuardJian);
                            break;
                        case 7:
                            identities.Add(Identities.GuardTi);
                            break;
                        case 8:
                            identities.Add(Identities.GuardZong);
                            break;
                        default:
                            identities.Add(Identities.Unknown);
                            break;
                    }
                }
                Identity = identities.ToArray();

                int msgTypeId = int.Parse(json["data"]["msg_type"].ToString());
                switch (msgTypeId)
                {
                    case 1:
                        MessageType = MessageTypes.Entry;
                        break;
                    case 2:
                        MessageType = MessageTypes.Attention;
                        break;
                    case 3:
                        MessageType = MessageTypes.Share;
                        break;
                    case 4:
                        MessageType = MessageTypes.SpecialAttention;
                        break;
                    case 5:
                        MessageType = MessageTypes.MutualAttention;
                        break;
                    default:
                        MessageType = MessageTypes.Unknown;
                        break;
                }

                TimeStamp = new DateTime(1970, 01, 01).AddSeconds(double.Parse(json["data"]["timestamp"].ToString()));
            }
        }

        [Serializable]
        public class RoomBlock : IItem
        {
            public Cmds Cmd => Cmds.ROOM_BLOCK_MSG;

            public User User { get; private set; }
            public uint Operator { get; private set; }

            public RoomBlock(JToken json)
            {
                User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["uname"].ToString()));
                Operator = uint.Parse(json["data"]["operator"].ToString());
            }
        }

        [Serializable]
        public class GuardBuy : ITimeStampedItem
        {
            public Cmds Cmd => Cmds.GUARD_BUY;
            public DateTime TimeStamp { get; private set; }

            public User User { get; private set; }
            public uint GuardLevel { get; private set; }
            public string GiftName { get; private set; }

            public GuardBuy(JToken json)
            {
                User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["username"].ToString()));
                GuardLevel = uint.Parse(json["data"]["guard_level"].ToString());
                GiftName = json["data"]["gift_name"].ToString();

                TimeStamp = new DateTime(1970, 01, 01).AddSeconds(double.Parse(json["data"]["start_time"].ToString()));
            }
        }

        public static IItem Parse(JToken json)
        {
            //Console.WriteLine(json.ToString());
            try
            {
                string[] cmd = (json["cmd"].ToString()).Split(':');
                switch (cmd[0])
                {
                    case "DANMU_MSG":
                        return new Danmaku(json);
                    case "SUPER_CHAT_MESSAGE":
                        return new SuperChat(json);
                    case "SEND_GIFT":
                        return new Gift(json);
                    case "COMBO_SEND":
                        return new ComboSend(json);
                    case "WELCOME":
                        return new Welcome(json);
                    case "WELCOME_GUARD":
                        return new WelcomeGuard(json);
                    case "GUARD_BUY":
                        return new GuardBuy(json);
                    case "INTERACT_WORD":
                        return new InteractWord(json);
                    case "ROOM_BLOCK_MSG":
                        return new RoomBlock(json);
                    case "LIVE":
                    case "PREPARING":
                    case "SPECIAL_GIFT":
                    case "USER_TOAST_MSG":
                    case "GUARD_MSG":
                    case "GUARD_LOTTERY_START":
                    case "ENTRY_EFFECT":
                    case "SYS_MSG":
                    case "ROOM_RANK":
                    case "TV_START":
                    case "NOTICE_MSG":
                    case "SYS_GIFT":
                    case "ROOM_REAL_TIME_MESSAGE_UPDATE":
                        return new Raw((Cmds)Enum.Parse(typeof(Cmds), json["cmd"].ToString()), json);
                    default:
                        return new Raw(Cmds.UNKNOW, json);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
    }
}
