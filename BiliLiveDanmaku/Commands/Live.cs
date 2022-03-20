using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliLive.Commands
{
    public class Live : ITimeStampedCommand
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

        public DateTime TimeStamp { get; private set; }
        public string LiveKey { get; private set; }
        public string VoiceBackground { get; private set; }
        public string SubSessionKey { get; private set; }
        public string LivePlatform { get; private set; }
        public int LiveModel { get; private set; }
        public uint RoomId { get; private set; }

        public CommandType CommandType => CommandType.LIVE;

        public string RawData { get; private set; }
        
        public Live(JToken json)
        {
            RawData = json.ToString(Newtonsoft.Json.Formatting.None);
            LiveKey = json["live_key"].ToString();
            VoiceBackground = json["voice_background"].ToString();
            SubSessionKey = json["sub_session_key"].ToString();
            LivePlatform = json["live_platform"].ToString();
            LiveModel = int.Parse(json["live_model"].ToString());
            RoomId = uint.Parse(json["roomid"].ToString());

            TimeStamp = new DateTime(1970, 01, 01).AddSeconds(double.Parse(json["live_time"].ToString()));
        }
    }
}
