using Newtonsoft.Json.Linq;
using System;

namespace BiliLive.Commands
{
    public class Gift : Command
    {
        public override CommandType CommandType => CommandType.SEND_GIFT;

        //public DateTime TimeStamp { get; private set; }

        public uint UID { get; }
        public string Username { get; }

        public string GiftName { get; private set; }
        public uint Number { get; private set; }
        public string FaceUri { get; private set; }
        public uint GiftId { get; private set; }
        public string Action { get; private set; }
        public string CoinType { get; private set; }
        public Gift(JToken json) : base(json)
        {
            GiftName = GetValue<string>("data", "giftName");
            Number = GetValue<uint>("data", "num");
            UID = GetValue<uint>("data", "uid");
            Username = GetValue<string>("data", "uname");
            FaceUri = GetValue<string>("data", "face");
            GiftId = GetValue<uint>("data", "giftId"); 
            Action = GetValue<string>("data", "action"); 
            CoinType = GetValue<string>("data", "coin_type");
            //TimeStamp = GetTimeStamp(GetValue<double>("data", "timestamp"));
        }
    }
}
