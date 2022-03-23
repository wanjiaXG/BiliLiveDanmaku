using Newtonsoft.Json.Linq;
using System;

namespace BiliLive.Commands
{
    public class SuperChat : Command
    {
        public uint Price { get; private set; }
        public string Message { get; private set; }
        public bool TransMark { get; private set; }
        public string MessageTrans { get; private set; }
        public uint UID { get; }
        public string Username { get; }
        public string Face { get; private set; }
        public TimeSpan Duration { get; private set; }

        public SuperChat(JToken json) : base(json)
        {
            Price = GetValue<uint>("data", "price");
            Message = GetValue<string>("data", "message");
            TransMark = GetValue<int>("data", "trans_mark") != 0;
            MessageTrans = GetValue<string>("data", "message_trans");
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            Face = GetValue<string>("data", "user_info", "face");
            Duration = TimeSpan.FromSeconds(GetValue<double>("data", "time"));
        }
    }
}
