using Newtonsoft.Json.Linq;

//已检查无运行异常
namespace BiliLive.Commands
{

    public class Preparing : Command
    {
        public uint RoomId { get; private set; }
        public Preparing(JToken json) : base(json)
        {
            RoomId = GetValue<uint>("roomid");
        }
    }
}
