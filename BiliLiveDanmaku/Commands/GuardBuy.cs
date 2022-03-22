using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class GuardBuy : Command
    {
        public override CommandType CommandType => CommandType.GUARD_BUY;
        //public DateTime TimeStamp { get; private set; }

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
            //TimeStamp = GetTimeStamp(GetValue<long>("data", "start_time"));
        }
    }
}
