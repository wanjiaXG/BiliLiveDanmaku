using Newtonsoft.Json.Linq;

namespace BiliLive.Commands
{
    public class Welcome : Command
    {
        public uint UID { get; }
        public string Username { get; }
        public bool Svip { get; private set; }
        public Welcome(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            Svip = GetValue<int>("data", "svip") != 0;
        }
    }
}
