using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class Live : Command
    {
        /*        [2022-03-20 14:49:03]: JsonsRecieved: {
          "cmd": "LIVE",
          "live_key": "231378916256965397",
          "voice_background": "",
          "sub_session_key": "231378916256965397sub_time:1647758940",
          "live_platform": "pc",
          "live_model": 0,
          "live_time": 1647758940,
          "roomid": 189205
        }*/

        //public DateTime TimeStamp { get; private set; }
        public string LiveKey { get; private set; }
        public string VoiceBackground { get; private set; }
        public string SubSessionKey { get; private set; }
        public string LivePlatform { get; private set; }
        public int LiveModel { get; private set; }
        public uint RoomId { get; private set; }

        public override CommandType CommandType => CommandType.LIVE;
        
        public Live(JToken json) : base(json)
        {
            LiveKey = GetValue<string>("live_key");
            VoiceBackground = GetValue<string>("voice_background");
            SubSessionKey = GetValue<string>("sub_session_key");
            LivePlatform = GetValue<string>("live_platform");
            LiveModel = GetValue<int>("live_model");
            RoomId = GetValue<uint>("roomid");
            //TimeStamp = GetTimeStamp(GetValue<long>("live_time"));
        }
    }
}
