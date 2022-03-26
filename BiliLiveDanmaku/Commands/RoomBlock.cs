using Newtonsoft.Json.Linq;

//已检查无运行异常
namespace BiliLive.Commands
{
    public class RoomBlock : Command
    {
        public uint UID { get; }
        public string Username { get; }

        public uint Operator { get; private set; }

        public RoomBlock(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            Operator = GetValue<uint>("data", "operator");
        }
    }
}
