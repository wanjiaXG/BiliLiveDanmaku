using Newtonsoft.Json.Linq;

namespace BiliLive.Commands
{

    public class Preparing : ICommand
    {
        public uint RoomId { get; private set; }

        public CommandType CommandType => CommandType.WATCHED_CHANGE;

        public string RawData { get; private set; }
        

        public Preparing(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            RoomId = uint.Parse(json["roomid"].ToString());
        }
    }
}
