using Newtonsoft.Json.Linq;

namespace BiliLive.Commands
{
    public class WatchedChanged : Command
    {
        public uint Count { get; private set; }
        public WatchedChanged(JToken json) : base(json)
        {
            Count = GetValue<uint>("data", "num");
        }
    }
}