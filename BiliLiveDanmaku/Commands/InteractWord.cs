using BiliLive.Commands.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

//已检查无运行异常
namespace BiliLive.Commands
{
    public class InteractWord : Command
    {
        public uint UID { get; }
        public string Username { get; }
        public ICollection<Identities> Identity { get; private set; }
        public MessageTypes MessageType { get; private set; }

        public InteractWord(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            List<Identities> identities = new List<Identities>();
            JArray list = GetValue<JArray>("data", "identities");
            foreach (var item in list)
            {
                identities.Add(Util.GetJTokenValue<Identities>(item));
            }
            Identity = identities.ToArray();
            MessageType = GetValue<MessageTypes>("data", "msg_type");
        }
    }
}
