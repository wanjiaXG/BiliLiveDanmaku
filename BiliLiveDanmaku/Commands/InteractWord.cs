using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    [Serializable]
    public class InteractWord : ITimeStampedCommand
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

        public CommandType CommandType => CommandType.INTERACT_WORD;
        public DateTime TimeStamp { get; private set; }

        public User User { get; private set; }
        public ICollection<Identities> Identity { get; private set; }
        public MessageTypes MessageType { get; private set; }

        public string RawData { get; private set; }

        public InteractWord(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
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
}
