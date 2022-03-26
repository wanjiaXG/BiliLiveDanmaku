using Newtonsoft.Json.Linq;

//已检查无运行异常
namespace BiliLive.Commands
{
    public class ComboSend : Command
    {
        public uint UID { get; }
        public string Username { get; }
        public string GiftName { get; private set; }
        public uint TotalNumber { get; private set; }
        public uint GiftId { get; private set; }
        public string Action { get; private set; }

        public ComboSend(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            GiftName = GetValue<string>("data", "gift_name");
            TotalNumber = GetValue<uint>("data", "total_num");
            GiftId = GetValue<uint>("data", "gift_id");
            Action = GetValue<string>("data", "action");
        }
    }
}
