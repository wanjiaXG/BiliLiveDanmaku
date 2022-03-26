using Newtonsoft.Json.Linq;

//已检查无运行异常
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