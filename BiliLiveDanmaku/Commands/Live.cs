using Newtonsoft.Json.Linq;

//已检查无运行异常
namespace BiliLive.Commands
{
    public class Live : Command
    {
        public string LiveKey { get; private set; }
        public string VoiceBackground { get; private set; }
        public string SubSessionKey { get; private set; }
        public string LivePlatform { get; private set; }
        public int LiveModel { get; private set; }
        public uint RoomId { get; private set; }
        public Live(JToken json) : base(json)
        {
            LiveKey = GetValue<string>("live_key");
            VoiceBackground = GetValue<string>("voice_background");
            SubSessionKey = GetValue<string>("sub_session_key");
            LivePlatform = GetValue<string>("live_platform");
            LiveModel = GetValue<int>("live_model");
            RoomId = GetValue<uint>("roomid");
        }
    }
}
