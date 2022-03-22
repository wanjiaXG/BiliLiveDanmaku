using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    [Serializable]
    public class WelcomeGuard : Command
    {
        public override CommandType CommandType => CommandType.WELCOME_GUARD;

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
