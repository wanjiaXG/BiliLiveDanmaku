using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BiliLive.Commands
{
    public class InteractWord : Command
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

        public override CommandType CommandType => CommandType.INTERACT_WORD;

        //public DateTime TimeStamp { get; private set; }

        public uint UID { get; }
        public string Username { get; }
        public ICollection<Identities> Identity { get; private set; }
        public MessageTypes MessageType { get; private set; }

        public InteractWord(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            List<Identities> identities = new List<Identities>();
            JToken list = GetValue<JToken>("data", "identities");
            if(list != null)
            {
                foreach (JToken item in list)
                {
                    identities.Add(GetJTokenValue<Identities>(item));
                }
            }
            Identity = identities.ToArray();
            MessageType = GetValue<MessageTypes>("data", "msg_type");
            //TimeStamp = GetTimeStamp(GetValue<long>("data", "timestamp"));
        }
    }
}
