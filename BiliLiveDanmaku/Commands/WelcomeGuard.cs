using Newtonsoft.Json.Linq;
using System;

//已检查无运行异常
namespace BiliLive.Commands
{
    public class WelcomeGuard : Command
    {
        public DateTime TimeStamp { get; private set; }
        public uint UID { get; }
        public string Username { get; }
        public uint GuardLevel { get; private set; }
        public WelcomeGuard(JToken json) : base(json)
        {
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            GuardLevel = GetValue<uint>("data", "guard_level");
        }
    }
}
