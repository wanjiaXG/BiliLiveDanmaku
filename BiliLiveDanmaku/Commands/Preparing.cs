using Newtonsoft.Json.Linq;

namespace BiliLive.Commands
{

    public class Preparing : Command
    {
        public uint RoomId { get; private set; }

        public override CommandType CommandType => CommandType.PREPARING;

        public Preparing(JToken json) : base(json)
        {
            RoomId = GetValue<uint>("roomid");
        }
    }
}
