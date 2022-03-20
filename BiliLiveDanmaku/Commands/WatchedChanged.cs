using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace BiliLive.Commands
{
    public class WatchedChanged : ICommand
    {
        public CommandType CommandType => CommandType.WATCHED_CHANGE;

        public uint Count { get; private set; }

        public string RawData { get; private set; }
        
        public WatchedChanged(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            Count = uint.Parse(json["data"]["num"].ToString());
        }


        /*        {
          "cmd": "WATCHED_CHANGE",
          "data": {
            "num": 6,
            "text_small": "6",
            "text_large": "6人看过"
          }*/
    }


}