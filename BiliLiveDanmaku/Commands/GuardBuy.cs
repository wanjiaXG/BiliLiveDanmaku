using Newtonsoft.Json.Linq;

//已检查无运行异常
namespace BiliLive.Commands
{
    public class GuardBuy : Command
    {
        public uint UID { get; }
        public string Username { get; }
        public uint GuardLevel { get; private set; }
        public string GiftName { get; private set; }
        public GuardBuy(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            GuardLevel = GetValue<uint>("data", "guard_level");
            GiftName = GetValue<string>("data", "gift_name");
        }
    }
}
