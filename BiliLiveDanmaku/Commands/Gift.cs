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
    public class Gift : ITimeStampedCommand
    {
        public CommandType CommandType => CommandType.SEND_GIFT;
        public DateTime TimeStamp { get; private set; }

        public string GiftName { get; private set; }
        public uint Number { get; private set; }
        public User User { get; private set; }
        public string FaceUri { get; private set; }
        public uint GiftId { get; private set; }
        public string Action { get; private set; }
        public string CoinType { get; private set; }

        public string RawData { get; private set; }
        public Gift(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            GiftName = Regex.Unescape(json["data"]["giftName"].ToString());
            Number = uint.Parse(json["data"]["num"].ToString());
            User = new User(uint.Parse(json["data"]["uid"].ToString()), Regex.Unescape(json["data"]["uname"].ToString()));
            FaceUri = json["data"]["face"].ToString();
            GiftId = uint.Parse(json["data"]["giftId"].ToString());
            Action = json["data"]["action"].ToString();
            CoinType = json["data"]["coin_type"].ToString();

            TimeStamp = new DateTime(1970, 01, 01).AddSeconds(double.Parse(json["data"]["timestamp"].ToString()));
        }
    }
}
