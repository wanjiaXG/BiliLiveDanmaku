using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace BiliLive.Commands
{
    public class WatchedChanged : Command
    {
        public override CommandType CommandType => CommandType.WATCHED_CHANGE;

        public uint Count { get; private set; }
        
        public WatchedChanged(JToken json) : base(json)
        {
            Count = GetValue<uint>("data", "num");
        }
    }


}