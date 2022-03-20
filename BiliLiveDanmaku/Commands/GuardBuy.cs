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
    public class GuardBuy : ITimeStampedCommand
    {
        public CommandType CommandType => CommandType.GUARD_BUY;
        public DateTime TimeStamp { get; private set; }

        public User User { get; private set; }
        public uint GuardLevel { get; private set; }
        public string GiftName { get; private set; }
        public string RawData { get; private set; }
        public GuardBuy(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["username"].ToString()));
            GuardLevel = uint.Parse(json["data"]["guard_level"].ToString());
            GiftName = json["data"]["gift_name"].ToString();

            TimeStamp = new DateTime(1970, 01, 01).AddSeconds(double.Parse(json["data"]["start_time"].ToString()));
        }
    }
}
