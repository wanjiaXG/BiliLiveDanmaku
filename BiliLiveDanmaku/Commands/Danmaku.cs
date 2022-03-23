using Newtonsoft.Json.Linq;

namespace BiliLive.Commands
{
    public class Danmaku : Command
    {
        public uint UID { get; }
        public string Username { get; }

        public string Message { get; private set; }

        public Danmaku(JToken json) : base(json)
        {

            UID = GetValue<uint>("info", 2, 0);
            Username = GetValue<string>("info", 2, 1);
            Message = GetValue<string>("info", 1);
        }
    }
}
